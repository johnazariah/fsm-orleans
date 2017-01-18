namespace FSM.Orleans

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open BrightSword.RoslynWrapper
open CSharp.UnionTypes

[<AutoOpen>]
module GrainImplementationDeclarationBuilder =
    let private state_to_state_processor_name s =
        sprintf "%sStateProcessor" s.StateName.unapply

    let to_processor_map vm =
        let sm = StateMachine vm

        let returnType =
            sprintf "Func<%s, %s, Task<%s>>" sm.grain_state_typename sm.message_typename sm.grain_state_typename

        let matchMethodOnState =
            sprintf "state.Match<%s>" returnType |> ident

        let methodArguments =
            vm.StateDefinitions
            |> Seq.map (state_to_state_processor_name >> ident >> SyntaxFactory.ParenthesizedLambdaExpression)
            |> Seq.map (fun x -> x :> ExpressionSyntax)

        in
        [
            ``arrow_method`` returnType "GetProcessorFunc" ``<<`` [] ``>>``
                ``(`` [ ("state", ``type`` sm.state_typename) ] ``)``
                [``protected``; ``override``]
                (Some (``=>`` (``invoke`` matchMethodOnState ``(`` methodArguments ``)``)))
            :> MemberDeclarationSyntax
        ]

    let to_message_endpoints vm =
        let sm = StateMachine vm

        let to_message_endpoint msg =
            let mt_to_param_name (mt: MessageType) = toParameterName mt.unapply
            let mt_to_param_specification (mt: MessageType) = ((mt_to_param_name mt), ``type`` mt.unapply)

            let methodParams =
                msg.MessageType
                |> Option.map mt_to_param_specification
                |> Option.fold (fun _ s -> [s]) []

            let message_name = sprintf "%sMessage" msg.MessageName.unapply

            let call =
                match msg.MessageType with
                | Some m -> let arg = m |> (mt_to_param_name >> ident) in (ident sm.message_typename) <.> (message_name, [ arg ])
                | None -> (ident sm.message_typename) <|.|> message_name

            let methodType = sprintf "Task<%sData>" sm.machine_name
            in
            ``arrow_method`` methodType msg.MessageName.unapply ``<<`` [] ``>>``
                ``(`` methodParams ``)``
                [``public``; ``async``]
                (Some (``=>`` (``await`` (``invoke`` (ident "ProcessMessage") ``(`` [ call ] ``)``))))
            :> MemberDeclarationSyntax
        in
        vm.MessageBlock.Messages |> List.map to_message_endpoint

    let to_state_processors vm =
        let sm = StateMachine vm

        let to_state_processor sd =
            let state_name = sd.StateName.unapply
            let interfaceName  = sprintf "I%sStateMessageHandler" state_name
            let className = sprintf "%sStateMessageHandler" state_name
            let delegatorClassName = sprintf "%sStateMessageDelegator" state_name

            let to_state_processor_method =
                let delegatorClassName = state_name |> sprintf "%sStateMessageDelegator"

                let dispatchLambda messageName =
                    if sd.StateTransitions |> Seq.map (fun st -> (st.unapply |> fst)) |> Seq.contains (messageName) then
                        let delegateMethodName = sprintf "Handle%sState%sMessage" state_name messageName
                        ident delegatorClassName <.> (delegateMethodName,  [ident "state"])
                    else
                        ident "HandleInvalidMessage" :> ExpressionSyntax

                let methodType = sprintf "Task<%s>" sm.grain_state_typename
                let methodName = sprintf "%sStateProcessor" state_name
                let args = [ ("state", ``type`` sm.grain_state_typename); ("message", ``type`` sm.message_typename) ]
                let dispatchArgs = vm.MessageBlock.Messages |> Seq.map (fun msg -> msg.MessageName.unapply |> dispatchLambda)
                in
                ``arrow_method`` methodType methodName ``<<`` [] ``>>``
                    ``(`` args ``)``
                    [``private``; ``static``; ``async``]
                    (Some (``=>`` (``await`` (``invoke`` (ident "message.Match") ``(`` dispatchArgs ``)``))))
                :> MemberDeclarationSyntax

            let to_message_handler_interface =
                let toStateMessageHandler (name, _) =
                    let args =
                        vm.MessageBlock.Messages
                        |> Seq.filter (fun m -> m.MessageName.unapply = name)
                        |> Seq.map (fun m ->
                            m.MessageType
                            |> Option.map (fun m' -> (m'.unapply |> toParameterName, ``type`` m'.unapply)))
                        |> Seq.choose id
                        |> Seq.toList

                    let t = sprintf "Task<%sStateMessageHandler.%s%sResult>" state_name state_name name
                    in
                    ``arrow_method`` t name ``<<`` [] ``>>``
                        ``(`` (("state", ``type`` sm.grain_state_typename) :: args) ``)``
                        []
                        None
                    :> MemberDeclarationSyntax

                let members = seq {
                        yield! (sd.StateTransitions |> Seq.map (fun st -> toStateMessageHandler st.unapply))
                    }
                in
                ``interface`` interfaceName ``<<`` [] ``>>``
                    ``:`` []
                    [``private``]
                    ``{``
                         members
                    ``}``
                :> MemberDeclarationSyntax

            let to_message_handler_delegator =
                let handlerField =
                    ``field`` interfaceName "_handler" [``private``; ``static``; ``readonly``]
                        (Some (``:=`` (``new`` (``type`` className) ``(`` [] ``)``)))
                    :> MemberDeclarationSyntax

                let toDispatchLambda (messageName, argTypeMaybe) =
                    let parameterList =
                        argTypeMaybe
                        |> Option.map toParameterName
                        |> Option.fold (fun s a -> s @ [ a ]) [ "state" ]

                    let handlerMethod =
                        ``invoke`` ((ident "_handler") <|.|> messageName)  ``(`` (parameterList |> List.map ident) ``)``

                    let handlerWithCast =
                        let castMethod =
                            ``_ =>`` "result" (``cast`` sm.grain_state_typename (``((`` ((ident "result") <|.|> "Result") ``))``))
                        ``invoke`` (handlerMethod <|.|> "ContinueWith") ``(`` [ castMethod ] ``)``

                    let lambda =
                        ``=>`` (``() =>`` parameterList handlerWithCast)

                    let lambdaType = System.String.Join(", " , (argTypeMaybe ?+ [ (sprintf "Task<%s>" sm.grain_state_typename) ])) |> sprintf "Func<%s>"
                    let methodName = sprintf "Handle%sState%sMessage" state_name messageName
                    in
                    ``arrow_method`` lambdaType methodName ``<<`` [] ``>>``
                        ``(`` [ ("state", ``type`` sm.grain_state_typename) ] ``)``
                        [``public``; ``static``]
                        (Some lambda)
                    :> MemberDeclarationSyntax

                let dispatcher (messageName, argTypeMaybe) =
                    if sd.StateTransitions |> Seq.map (fun st -> (st.unapply |> fst)) |> Seq.contains (messageName) then
                        (messageName, argTypeMaybe) |> toDispatchLambda |> Some
                    else
                        None
                let members = seq {
                        yield handlerField
                        yield! vm.MessageBlock.Messages |> Seq.map (fun msg -> msg.unapply |> dispatcher) |> Seq.choose id
                    }
                ``class`` delegatorClassName ``<<`` [] ``>>``
                    ``:`` None ``,`` []
                    [``private``; ``static``]
                    ``{``
                        members
                    ``}``
                :> MemberDeclarationSyntax

            let to_message_handler_implementation =
                let toResultStateClassName = sprintf "%s%sResultState" state_name
                let toResultClassName = sprintf "%s%sResult" state_name

                let toResultStateClass (name, targets) =
                    {
                        UnionTypeName = UnionTypeName (name |> toResultStateClassName)
                        UnionTypeParameters = []
                        UnionMembers = targets |> List.map (sprintf "%sState" >> (fun s -> (UnionMemberName s, None)) >> UnionMember.apply)
                        BaseType = FullTypeName.apply((sm.state_typename, []), None) |> Some
                    }
                    |> to_class_declaration

                let toResultClass (name, _) =
                    let className = name |> toResultClassName
                    let transitionResultState = name |> toResultStateClassName
                    let baseClassName =
                        sprintf "StateMachineGrainState<%s, %s>.StateTransitionResult<%s>" sm.data_typename sm.state_typename transitionResultState
                        |> Some
                    in
                    let ctor =
                        ``constructor`` className
                            ``(`` [("stateMachineData", ``type`` sm.data_typename); ("stateMachineState", ``type`` transitionResultState)]``)``
                            ``:`` ["stateMachineData"; "stateMachineState"]
                            [``public``]
                            ``{`` [] ``}``
                        :> MemberDeclarationSyntax

                    let explicitCastToBase =
                        let dataArg = ident "value.StateMachineData" :> ExpressionSyntax
                        let castStateArg = ``cast`` sm.state_typename (ident "value.StateMachineState") :> ExpressionSyntax
                        in
                        ``explicit operator`` sm.grain_state_typename ``(`` (``type`` className) ``)``
                            (``=>`` (``new`` (``type`` sm.grain_state_typename) ``(`` [dataArg; castStateArg] ``)``))
                    in
                    ``class`` className ``<<`` [] ``>>``
                        ``:`` baseClassName ``,`` []
                        [``public``]
                        ``{``
                            [
                                ctor
                                explicitCastToBase
                            ]
                        ``}``
                    :> MemberDeclarationSyntax

                let members = seq {
                        yield! (sd.StateTransitions |> Seq.map (fun st -> toResultStateClass st.unapply))
                        yield! (sd.StateTransitions |> Seq.map (fun st -> toResultClass st.unapply))
                    }
                in
                ``class`` className ``<<`` [] ``>>``
                    ``:`` None ``,`` [ interfaceName]
                    [``private``; ``partial``]
                    ``{``
                        members
                    ``}``
                :> MemberDeclarationSyntax
            in
            [
                to_state_processor_method
                to_message_handler_interface
                to_message_handler_delegator
                to_message_handler_implementation
            ]
        in
        vm.StateDefinitions |> List.collect to_state_processor

    let build_grain_implementation_with_member_generators fns vm =
        let sm = StateMachine vm
        let members = fns |> Seq.collect (fun f -> vm |> (f >> List.toSeq))
        [
            ``class`` sm.machine_name ``<<`` [] ``>>``
                ``:`` (Some sm.grain_impl_base_typename) ``,`` [ sm.grain_interface_base_typename ]
                [``public``; ``partial``]
                ``{``
                    members
                ``}``
                :> MemberDeclarationSyntax
        ]

    let build_grain_implementation =
        let fns =
            [
                to_processor_map
                to_message_endpoints
                to_state_processors
            ]
        in
        build_grain_implementation_with_member_generators fns

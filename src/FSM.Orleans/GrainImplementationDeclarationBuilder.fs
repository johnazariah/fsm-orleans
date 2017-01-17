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

        let stateTypeName = ``type`` sm.grain_state_typename

        let methodArguments = 
            vm.StateDefinitions 
            |> Seq.map (state_to_state_processor_name >> ident >> SyntaxFactory.ParenthesizedLambdaExpression)
            |> Seq.map (fun x -> x :> ExpressionSyntax)            
        
        in
        [
            ``arrow_method`` returnType "GetProcessorFunc" ``<<`` [] ``>>``
                ``(`` [ ("state", stateTypeName) ] ``)``
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

            let methodInvokeArguments =
                msg.MessageType
                |> Option.map (mt_to_param_name >> ident)
                |> Option.fold (fun _ s -> [s]) []

            let methodBody =
                let delegator = 
                    let methodName = ident (sprintf "%sMessage.%s" sm.machine_name msg.MessageName.unapply)
                    ``invoke`` methodName ``(`` methodInvokeArguments ``)``
                in
                ``await`` (``invoke`` (ident "ProcessMessage") ``(`` [ delegator ] ``)``)

            let methodType = sprintf "Task<%sData>" sm.machine_name            
            in
            ``arrow_method`` methodType msg.MessageName.unapply ``<<`` [] ``>>``
                ``(`` methodParams ``)``
                [``public``; ``async``]
                (Some (``=>`` methodBody))
            :> MemberDeclarationSyntax
        in 
        vm.MessageBlock.Messages |> List.map to_message_endpoint


    let build_implementation_class_with_member_generators fns vm = 
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
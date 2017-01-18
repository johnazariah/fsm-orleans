namespace Parser.FSM.Orleans

[<AutoOpen>]
module CodeGen_GrainImplementationModule =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    open BrightSword.CSharpExtensions.DiscriminatedUnion.Model
    open BrightSword.CSharpExtensions.DiscriminatedUnion.CodeGenerator
    
    open BrightSword.RoslynWrapper.Common
    open BrightSword.RoslynWrapper.FieldDeclaration
    open BrightSword.RoslynWrapper.MethodDeclaration
    open BrightSword.RoslynWrapper.PropertyDeclaration
    open BrightSword.RoslynWrapper.ConstructorDeclaration
    open BrightSword.RoslynWrapper.InterfaceDeclaration
    open BrightSword.RoslynWrapper.ClassDeclaration
    open BrightSword.RoslynWrapper.NamespaceDeclaration
    open BrightSword.RoslynWrapper.CompilationUnit
    open BrightSword.RoslynWrapper.Invocation
    open BrightSword.RoslynWrapper.ObjectCreation
    open BrightSword.RoslynWrapper.Conversion

    let toGeneralPartial vm = 
        let machineName = vm.MachineName.unapply
        let grainStateClass = machineName |> sprintf "%sGrainState"
        let dataClass       = machineName |> sprintf "%sData"
        let stateClass      = machineName |> sprintf "%sState"
        let messageClass    = machineName |> sprintf "%sMessage"
        let baseMachineGrainClass = sprintf "StateMachineGrain<%s,%s,%s,%s>" grainStateClass dataClass stateClass messageClass
        let baseMachineInterface = machineName |> sprintf "I%s"

        let toPublicMessageEndpoint msg = 
            let methodParams = 
                msg.MessageType 
                |> Option.map (fun mt -> (mt.unapply |> toParameterName, mt.unapply)) 
                |> Option.fold (fun _ s -> [s]) []

            let methodInvokeArguments =
                msg.MessageType
                |> Option.map (fun mt -> mt.unapply |> toParameterName)
                |> Option.map (SF.IdentifierName >> SF.Argument)
                |> Option.fold (fun _ s -> [s]) []

            let methodBody =
                let delegator = 
                    let methodName = (sprintf "%sMessage.%s" machineName msg.MessageName.unapply) |> toIdentifierName
                    ``invoke`` methodName ``(`` methodInvokeArguments ``)``
                    |> SF.Argument
                let m =  "ProcessMessage" |> toIdentifierName
                in
                ``=>`` (``await`` (``invoke`` m ``(`` [ delegator ] ``)``))
                |> Some

            let methodType = sprintf "Task<%sData>" machineName
            ``method`` methodType msg.MessageName.unapply ``<<`` [] ``>>``
                ``(`` methodParams ``)``
                [``public``; ``async``]
                ``=>`` methodBody
            :> MemberDeclarationSyntax

        let processorMapFunction =
            let returnType = (sprintf "Func<%s, %s, Task<%s>>" grainStateClass messageClass grainStateClass)
            let methodArguments = 
                vm.StateDefinitions 
                |> Seq.map (fun s -> (s.StateName.unapply |> sprintf "%sStateProcessor"))
                |> Seq.map (toIdentifierName >> SF.ParenthesizedLambdaExpression >> SF.Argument)
            let methodBody = 
                let m =  (sprintf "state.Match<%s>" returnType) |> toIdentifierName
                ``=>`` (``invoke`` m ``(`` methodArguments ``)``) |> Some

            ``method`` returnType "GetProcessorFunc" ``<<`` [] ``>>``
                ``(`` [ ("state", stateClass) ] ``)``
                [``protected``; ``override``]
                ``=>`` methodBody
            :> MemberDeclarationSyntax

        let members = seq {
                yield processorMapFunction
                yield! vm.MessageBlock.Messages |> Seq.map toPublicMessageEndpoint
            }

        ``class`` machineName ``<<`` [] ``>>``
            ``:`` (Some baseMachineGrainClass) ``,`` [ baseMachineInterface ]
            [``public``; ``partial``]
            ``{``
                members
            ``}``

    let toStateProcessorPartial vm sd =
        let machineName = vm.MachineName.unapply
        let grainStateClass = machineName |> sprintf "%sGrainState"
        let dataClass       = machineName |> sprintf "%sData"
        let stateClass      = machineName |> sprintf "%sState"
        let messageClass    = machineName |> sprintf "%sMessage"
        
        let stateName = sd.StateName.unapply
        let interfaceName      = stateName |> sprintf "I%sStateMessageHandler"
        let className          = stateName |> sprintf "%sStateMessageHandler"
        let delegatorClassName = stateName |> sprintf "%sStateMessageDelegator"

        let stateProcessorMethod = 
            let dispatchLambda messageName =
                if sd.StateTransitions |> Seq.map (fun st -> (st.unapply |> fst)) |> Seq.contains (messageName) then
                    let delegateMethodName = sprintf "Handle%sState%sMessage" stateName messageName
                    let delegateMethodArgs = ["state"] |> toArgumentList
                    ``invoke`` ((toIdentifierName delegatorClassName) <.> (toIdentifierName delegateMethodName)) ``(`` delegateMethodArgs ``)`` 
                    |> SF.Argument
                else
                    "HandleInvalidMessage" |> (toIdentifierName >> SF.Argument)

            let methodType = sprintf "Task<%s>" grainStateClass
            let methodName = sprintf "%sStateProcessor" stateName
            let args = [ ("state", grainStateClass); ("message", messageClass) ]
            let dispatchArgs = vm.MessageBlock.Messages |> Seq.map (fun msg -> msg.MessageName.unapply |> dispatchLambda)
            let dispatchExpression = 
                let m = "message.Match" |> toIdentifierName
                ``=>`` (``await`` (``invoke`` m ``(`` dispatchArgs ``)``))
            in
            ``method`` methodType methodName ``<<`` [] ``>>``
                ``(`` args ``)``
                [``private``; ``static``; ``async``]
                ``=>`` (Some dispatchExpression)
            :> MemberDeclarationSyntax

        let messageHandlerInterface = 
            let toStateMessageHandler (name, _) =
                let args = 
                    vm.MessageBlock.Messages 
                    |> Seq.filter (fun m -> m.MessageName.unapply = name) 
                    |> Seq.map (fun m -> 
                        m.MessageType 
                        |> Option.map (fun m' -> (m'.unapply |> toParameterName, m'.unapply)))
                    |> Seq.choose id
                    |> Seq.toList

                let t = sprintf "Task<%sStateMessageHandler.%s%sResult>" stateName stateName name
                in 
                ``method`` t name ``<<`` [] ``>>``
                    ``(`` (("state", grainStateClass) :: args) ``)``
                    []
                    ``=>`` None
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

        let messageHandlerDelegator = 
            let handlerField = 
                let initializer =  ``:=`` (``new`` [className] ``(`` [] ``)``)
                in
                ``field`` interfaceName "_handler" [``private``; ``static``; ``readonly``] ``:=`` (Some initializer)
                :> MemberDeclarationSyntax
            
            let toDispatchLambda (messageName, argTypeMaybe) =
                let parameterList = 
                    argTypeMaybe 
                    |> Option.map toParameterName
                    |> Option.fold (fun _ a -> [ a ]) []

                let handlerMethod = 
                    ``invoke`` ((toIdentifierName "_handler") <.> (toIdentifierName messageName)) 
                        ``(`` ("state" :: parameterList |> toArgumentList) ``)``

                let handlerWithCast = 
                    let castMethod = 
                        ``_ =>`` "result" (``cast`` grainStateClass (``(`` ((toIdentifierName "result") <.> (toIdentifierName "Result")) ``)``)) 
                        |> SF.Argument
                    ``invoke`` (handlerMethod <.> (toIdentifierName "ContinueWith")) ``(`` [ castMethod ] ``)``

                let lambda = 
                    ``=>`` (``() =>`` parameterList handlerWithCast)

                let lambdaType = System.String.Join("," , (argTypeMaybe ?+ [ (sprintf "Task<%s>" grainStateClass) ])) |> sprintf "Func<%s>"
                let methodName = sprintf "Handle%sState%sMessage" stateName messageName
                in 
                ``method`` lambdaType methodName ``<<`` [] ``>>`` 
                    ``(`` [ ("state", grainStateClass) ] ``)`` 
                    [``public``; ``static``]  
                    ``=>`` (Some lambda)
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
            
        let messageHandlerImplementationPartial =
            let toResultStateClassName = sprintf "%s%sResultState" stateName             
            let toResultClassName = sprintf "%s%sResult" stateName 

            let toResultStateClass (name, targets) =
                let du = (name |> toResultStateClassName, targets |> Seq.map (sprintf "%sState"), stateClass) |> DiscriminatedUnion.apply
                du |> toClassDeclaration :> MemberDeclarationSyntax

            let toResultClass (name, _) =
                let className = name |> toResultClassName
                let transitionResultState = name |> toResultStateClassName
                let baseClassName = 
                    sprintf "StateMachineGrainState<%s, %s>.StateTransitionResult<%s>" dataClass stateClass transitionResultState 
                    |> Some
                in
                let ctor = 
                    ``constructor`` className 
                        ``(`` [("stateMachineData",dataClass); ("stateMachineState",transitionResultState)]``)``
                        ``:`` ["stateMachineData"; "stateMachineState"]
                        [``public``]
                        ``{`` [] ``}``
                    :> MemberDeclarationSyntax

                let explicitCastToBase =
                    let castExpr = sprintf "(%s)result.StateMachineState" stateClass
                    let initializer = ``=>`` (``new`` [ grainStateClass ] ``(`` ["result.StateMachineData"; castExpr] ``)``)
                    in
                    ``explicit operator`` grainStateClass ``(`` className ``)`` ``=>`` initializer
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

        let members = seq {
                yield stateProcessorMethod
                yield messageHandlerDelegator
                yield messageHandlerInterface
                yield messageHandlerImplementationPartial
            }
        in
        ``class`` machineName ``<<`` [] ``>>``
            ``:`` None ``,`` [ ]
            [``public``; ``partial``]
            ``{``
                members
            ``}``
        :> MemberDeclarationSyntax

    let toGrainState vm = 
        let machineName = vm.MachineName.unapply
        let grainStateClass = machineName |> sprintf "%sGrainState"
        let dataClass       = machineName |> sprintf "%sData"
        let stateClass      = machineName |> sprintf "%sState"
        let baseClass = sprintf "StateMachineGrainState<%s, %s>" dataClass stateClass |> Some
        let ctor = 
            ``constructor`` machineName ``(`` [("stateMachineData", dataClass);("stateMachineState", stateClass)] ``)``
                ``:`` ["stateMachineData"; "stateMachineState"]
                [``public``]
                ``{`` [] ``}``
            :> MemberDeclarationSyntax
        in
        ``class`` grainStateClass ``<<`` [] ``>>``
            ``:`` baseClass ``,`` []
            [``public``]
            ``{``
                [ ctor ]
            ``}``

    let generateGrainImplementationModule vm =  
        let namespaceMembers = seq {
            yield vm |> toGrainState :> MemberDeclarationSyntax
            yield vm |> toGeneralPartial :> MemberDeclarationSyntax 
            yield! (vm.StateDefinitions |> Seq.map (toStateProcessorPartial vm))
        }

        let namespaceWithMembers = 
            ``namespace`` (vm.MachineName.unapply |> sprintf "FSM.%s.Orleans")
                ``{`` 
                    []
                    namespaceMembers
                ``}``
        in
        ``compilation unit`` [ namespaceWithMembers ]
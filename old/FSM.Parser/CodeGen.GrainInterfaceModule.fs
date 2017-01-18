namespace Parser.FSM.Orleans

[<AutoOpen>]
module CodeGen_GrainInterfaceModule =
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

    let private getDataDU vm = 
        { DiscriminatedUnion.Zero with
            DiscriminatedUnionName = vm.MachineName.unapply |> sprintf "%sData"
            DiscriminatedUnionMembers = [ vm.DataBlock.unapply |> DiscriminatedUnionMember.apply ]
        }

    let private getMessageDU vm = 
        let toDUMember m = 
            let messageName = m.MessageName.unapply |> sprintf "%sMessage"
            match m.MessageType with
            | Some messageType -> (messageName, messageType.unapply) |> DiscriminatedUnionMember.apply
            | None -> messageName |> DiscriminatedUnionMember.apply
        in
        { DiscriminatedUnion.Zero with
            DiscriminatedUnionName = vm.MachineName.unapply |> sprintf "%sMessage"
            DiscriminatedUnionMembers = vm.MessageBlock.Messages |> List.map toDUMember
        }

    let private getStateDU vm = 
        { DiscriminatedUnion.Zero with
            DiscriminatedUnionName = vm.MachineName.unapply |> sprintf "%sState"
            DiscriminatedUnionMembers = vm.StateDefinitions |> List.map (fun sd -> sd.StateName.unapply |> sprintf "%sState" |> DiscriminatedUnionMember.apply)
        }

    let private getIGrainInterface vm = 
        let machineName = vm.MachineName.unapply 
        let grainDataType    = (machineName |> sprintf "%sData")
        let grainMessageType = (machineName |> sprintf "%sMessage")
        let interfaceName    = (machineName |> sprintf "I%s")
        let interfaceBase    = (sprintf "IStateMachineGrain<%s, %s>" grainDataType grainMessageType)
        let methodSignatures = 
            let toMethodSignature message = 
                let methodParameters = 
                    message.MessageType 
                    |> Option.map (fun m -> (m.unapply.ToLower(), m.unapply))
                    |> Option.fold (fun _ v -> [v]) []
                in
                ``method`` (sprintf "Task<%s>" grainDataType) message.MessageName.unapply ``<<`` [] ``>>``
                    ``(`` methodParameters ``)``           
                    [] 
                    ``=>`` None
                :> MemberDeclarationSyntax
            in
            vm.MessageBlock.Messages 
            |> Seq.map toMethodSignature
                           
        in
        ``interface`` interfaceName ``<<`` [] ``>>`` 
            ``:`` [ interfaceBase ]
            [``public``] 
            ``{`` 
                methodSignatures 
            ``}``

    let generateGrainInterfaceModule vm = 
        let namespaceMembers = seq {
                yield vm |> getDataDU |> toClassDeclaration :> MemberDeclarationSyntax              
                yield vm |> getStateDU |> toClassDeclaration :> MemberDeclarationSyntax
                yield vm |> getMessageDU |> toClassDeclaration :> MemberDeclarationSyntax
                yield vm |> getIGrainInterface :> MemberDeclarationSyntax
            }

        let namespaceWithMembers = 
            ``namespace`` (vm.MachineName.unapply |> sprintf "FSM.%s.Orleans")
                ``{`` 
                    []
                    namespaceMembers
                ``}``
        in
        ``compilation unit`` [ namespaceWithMembers ]

namespace FSM.Orleans

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open CSharp.UnionTypes
open BrightSword.RoslynWrapper

[<AutoOpen>]
module NamespaceBuilder = 
    let build_namespace_with_members members vm = 
        let sm = StateMachine vm
        in
        ``namespace`` sm.namespace_name
            ``{``
                []
                members
            ``}``
     
    let build_namespace_with_member_generators fns vm = 
        let members = fns |> Seq.collect (fun f -> vm |> (f >> List.toSeq))
        in 
        build_namespace_with_members members vm


namespace FSM.Orleans

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open CSharp.UnionTypes
open BrightSword.RoslynWrapper

[<AutoOpen>]
module NamespaceBuilder = 
    let to_namespace_builder_internal fns vm = 
        let sm = StateMachine vm
        let members = fns |> Seq.collect (fun f -> vm |> (f >> List.toSeq))
        in
        ``namespace`` sm.namespace_name
            ``{``
                []
                members
            ``}``


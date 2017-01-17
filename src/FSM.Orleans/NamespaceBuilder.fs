namespace FSM.Orleans

[<AutoOpen>]
module NamespaceBuilder =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open CSharp.UnionTypes
    open BrightSword.RoslynWrapper

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

    let build_namespace vm =
        let fns =
            [
                build_grain_interface
                build_data_union
                build_state_union
                build_message_union
                build_grain_state
                build_grain_implementation
            ]
        in
        build_namespace_with_member_generators fns vm
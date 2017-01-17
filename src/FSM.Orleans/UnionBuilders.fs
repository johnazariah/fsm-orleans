namespace FSM.Orleans

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open BrightSword.RoslynWrapper
open CSharp.UnionTypes

[<AutoOpen>]
module UnionBuilders =
    let build_data_union vm = 
        let sm = StateMachine vm
        [
            to_class_declaration sm.data_union
        ]

    let build_state_union vm =
        let sm = StateMachine vm
        [
            to_class_declaration sm.state_union
        ]

    let build_message_union vm = 
        let sm = StateMachine vm
        [
            to_class_declaration sm.message_union
        ]


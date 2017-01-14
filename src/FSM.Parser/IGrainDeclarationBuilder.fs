namespace FSM.Orleans

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open BrightSword.RoslynWrapper
open CSharp.UnionTypes

[<AutoOpen>]
module internal IGrainDeclarationBuilder =
    let build_grain_interface vm =
        let machine_name = vm.MachineName.unapply
        let grain_data_typename = sprintf "%sData" machine_name
        let grain_message_typename = sprintf "%sMessage" machine_name
        let grain_interface_typename = sprintf "I%s" machine_name
        let grain_interface_base_typename = sprintf "IStateMachineGrain<%s, %s>" grain_data_typename grain_message_typename

        let grain_message_handler_signatures =
            let to_grain_message_handler_signature message =
                let method_handler_type = sprintf "Task<%s>" grain_data_typename
                let method_handler_args = message.MessageType |> Option.map (fun m -> (m.unapply.ToLower(), ``type`` (m.unapply))) |> Option.fold (fun _ v -> [v]) []
                in
                ``arrow_method`` method_handler_type message.MessageName.unapply ``<<`` [] ``>>``
                    ``(`` method_handler_args ``)``
                    []
                    None
                :> MemberDeclarationSyntax
            in
            vm.MessageBlock.Messages
            |> List.map to_grain_message_handler_signature
        in
        ``interface`` grain_interface_typename ``<<`` [] ``>>``
            ``:`` [ grain_interface_base_typename ]
            [``public``]
            ``{``
                grain_message_handler_signatures
            ``}``


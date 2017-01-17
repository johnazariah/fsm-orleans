namespace FSM.Orleans

open CSharp.UnionTypes

[<AutoOpen>]
module Machine = 
    let private to_union_member member_name = UnionMember.apply(UnionMemberName member_name, None)

    let private to_typed_union_member (member_name, member_type) =
        UnionMember.apply(UnionMemberName member_name, Some <| FullTypeName.apply((member_type, []), None))

    let text_to_valid_machine = 
        parse_machine_from_text_or_fail >> validate_parsed_machine

    type StateMachine (vm) = class
        member this.machine_name = vm.MachineName.unapply

        member this.namespace_name = sprintf "FSM.%s.Orleans" this.machine_name

        member this.grain_interface_typename = sprintf "I%s" this.machine_name
        member this.grain_interface_base_typename = sprintf "IStateMachineGrain<%s, %s>" this.data_typename this.message_typename

        member this.grain_state_typename = sprintf "%sGrainState" this.machine_name

        member this.grain_impl_base_typename = sprintf "StateMachineGrain<%s,%s,%s,%s>" this.grain_state_typename this.data_typename this.state_typename this.message_typename

        member this.state_typename = sprintf "%sState" this.machine_name
        member this.state_union =
            {
                BaseType = None
                UnionTypeParameters = []
                UnionTypeName = vm.MachineName.unapply |> (sprintf "%sState" >> UnionTypeName)
                UnionMembers =
                    vm.StateDefinitions
                    |> List.map ((fun sd -> sprintf "%sState" sd.StateName.unapply) >> to_union_member)
            }

        member this.data_typename = sprintf "%sData" this.machine_name
        member this.data_union =
            {
                BaseType = None
                UnionTypeParameters = []
                UnionTypeName = vm.MachineName.unapply |> (sprintf "%sData" >> UnionTypeName)
                UnionMembers = [ vm.DataBlock.unapply |> to_typed_union_member ]
            }

        member this.message_typename = sprintf "%sMessage" this.machine_name
        member this.message_union =
            let to_message_union_member m =
                let messageName = m.MessageName.unapply |> sprintf "%sMessage"
                match m.MessageType with
                | Some messageType -> to_typed_union_member (messageName, messageType.unapply)
                | None -> to_union_member messageName
            {
                BaseType = None
                UnionTypeParameters = []
                UnionTypeName = vm.MachineName.unapply |> (sprintf "%sMessage" >> UnionTypeName)
                UnionMembers =
                    vm.MessageBlock.Messages
                    |> List.map (to_message_union_member)
            }
    end
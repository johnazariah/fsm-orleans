namespace FSM.Orleans

open CSharp.UnionTypes

[<AutoOpen>]
module internal UnionTypes =
    let private to_union_member member_name =
        UnionMember.apply(UnionMemberName member_name, None)

    let private to_typed_union_member (member_name, member_type) =
        UnionMember.apply(UnionMemberName member_name, Some <| FullTypeName.apply((member_type, []), None))

    let get_state_union vm : UnionType =
        {
            BaseType = None
            UnionTypeParameters = []
            UnionTypeName = vm.MachineName.unapply |> (sprintf "%sState" >> UnionTypeName)
            UnionMembers =
                vm.StateDefinitions
                |> List.map ((fun sd -> sprintf "%sState" sd.StateName.unapply) >> to_union_member)
        }

    let get_data_union vm =
        {
            BaseType = None
            UnionTypeParameters = []
            UnionTypeName = vm.MachineName.unapply |> (sprintf "%sData" >> UnionTypeName)
            UnionMembers = [ vm.DataBlock.unapply |> to_typed_union_member ]
        }

    let get_message_union vm =
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
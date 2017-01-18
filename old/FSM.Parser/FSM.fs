namespace Parser.FSM.Orleans

[<AutoOpen>]
module FSM = 
    type FSM = {
        InitialState      : StateName
        Identifier        : Identifier
        DataBlock         : DataBlock
        MessageBlock      : MessageBlock
        StateDefinitions  : StateDefinition list
    }

    let buildFsm (Machine (machineName, machineItems)) = {
        InitialState = machineItems |> List.choose (function | InitialState x -> Some x | _ -> None) |> List.exactlyOne 
        Identifier = machineItems |> List.choose (function | Identifier x -> Some x | _ -> None) |> List.exactlyOne
        DataBlock = machineItems |> List.choose (function | DataBlock x -> Some x | _ -> None) |> List.exactlyOne
        MessageBlock = machineItems |> List.choose (function | MessageBlock  x -> Some x | _ -> None) |> List.exactlyOne
        StateDefinitions = machineItems |> List.choose (function | StateDefinition  x -> Some x | _ -> None)
    }
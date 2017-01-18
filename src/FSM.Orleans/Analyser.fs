namespace FSM.Orleans

[<AutoOpen>]
module Analyser =
    type Machine = {
        MachineName       : MachineName
        InitialState      : StateName
        Identifier        : Identifier
        DataBlock         : DataBlock
        MessageBlock      : MessageBlock
        StateDefinitions  : StateDefinition list
    } with
        static member Zero = {
            MachineName       = MachineName.Zero
            InitialState      = StateName.Zero
            Identifier        = Identifier.Zero
            DataBlock         = DataBlock.Zero
            MessageBlock      = MessageBlock.Zero
            StateDefinitions  = []
        }

    let private ``buildValidMachine simple-but-inefficient`` (machineName, machineItems) : Machine = {
        MachineName      = machineName
        InitialState     = machineItems |> List.choose(function | InitialState    x -> Some x | _ -> None) |> List.head
        Identifier       = machineItems |> List.choose(function | Identifier      x -> Some x | _ -> None) |> List.head
        DataBlock        = machineItems |> List.choose(function | DataBlock       x -> Some x | _ -> None) |> List.head
        MessageBlock     = machineItems |> List.choose(function | MessageBlock    x -> Some x | _ -> None) |> List.head
        StateDefinitions = machineItems |> List.choose(function | StateDefinition x -> Some x | _ -> None)
    }

    let private ``buildValidMachine single-pass-explicit-match`` (machineName, machineItems) : Machine =
        let setProperty (vm : Machine) = function
        | InitialState    mi -> {vm with InitialState     = mi }
        | Identifier      mi -> {vm with Identifier       = mi }
        | DataBlock       mi -> {vm with DataBlock        = mi }
        | MessageBlock    mi -> {vm with MessageBlock     = mi }
        | StateDefinition mi -> {vm with StateDefinitions = mi :: vm.StateDefinitions}
        in
        machineItems |> List.fold setProperty { Machine.Zero with MachineName = machineName }

    let private ``buildValidMachine single-pass-composed-fold`` (machineName, machineItems) : Machine =
        let trySetProperty (selector, validator, setter) =
            fun vm mi ->
                (mi |> selector)
                |> Option.bind (validator vm)
                |> Option.map (setter vm)
                |> Option.fold (fun _ m -> m) vm

        let propertySetters =
            [
                trySetProperty ((function | InitialState    mi -> Some mi | _ -> None), (fun vm mi -> if (vm.InitialState = StateName.Zero)    then Some mi else None), (fun vm mi -> {vm with InitialState = mi }))
                trySetProperty ((function | Identifier      mi -> Some mi | _ -> None), (fun vm mi -> if (vm.Identifier   = Identifier.Zero)   then Some mi else None), (fun vm mi -> {vm with Identifier   = mi }))
                trySetProperty ((function | DataBlock       mi -> Some mi | _ -> None), (fun vm mi -> if (vm.DataBlock    = DataBlock.Zero)    then Some mi else None), (fun vm mi -> {vm with DataBlock    = mi }))
                trySetProperty ((function | MessageBlock    mi -> Some mi | _ -> None), (fun vm mi -> if (vm.MessageBlock = MessageBlock.Zero) then Some mi else None), (fun vm mi -> {vm with MessageBlock = mi }))
                trySetProperty ((function | StateDefinition mi -> Some mi | _ -> None), (fun _ mi -> Some mi), (fun vm mi -> {vm with StateDefinitions = mi :: vm.StateDefinitions}))
            ]

        let setSingleProperty mi vm' f = f vm' mi
        let setProperty vm mi = propertySetters |> List.fold (setSingleProperty mi) vm
        in
        machineItems |> List.fold setProperty { Machine.Zero with MachineName = machineName }

    let validate_parsed_machine pm =
        (pm.ParsedMachineName, pm.ParsedMachineItems)
        |> ``buildValidMachine single-pass-composed-fold``
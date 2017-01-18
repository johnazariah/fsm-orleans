namespace Parser.FSM.Orleans

[<AutoOpen>]
module AST =
    type IdentifierName = | IdentifierName of string
    with
        static member Zero = IdentifierName ""
        member this.unapply = match this with | IdentifierName s -> s

    type IdentifierType = | IdentifierType of string
    with
        static member Zero = IdentifierType ""
        member this.unapply = match this with | IdentifierType s -> s

    type Identifier = {IdentifierName : IdentifierName; IdentifierType : IdentifierType }
    with 
        static member Zero = {IdentifierName = IdentifierName.Zero; IdentifierType = IdentifierType.Zero}
        static member apply(_name, _type) = {IdentifierName = _name; IdentifierType = _type}
        member this.unapply = (this.IdentifierName.unapply, this.IdentifierType.unapply)

    type DataElementName = | DataElementName of string
    with
        static member Zero = DataElementName ""
        member this.unapply = match this with | DataElementName s -> s

    type DataElementType = | DataElementType of string
    with
        static member Zero = DataElementType ""
        member this.unapply = match this with | DataElementType s -> s

    type DataBlock = {DataElementName : DataElementName; DataElementType : DataElementType}
    with
        static member Zero = {DataElementName = DataElementName.Zero; DataElementType = DataElementType.Zero}
        static member apply(_name, _type) = {DataElementName = _name; DataElementType = _type}
        member this.unapply = (this.DataElementName.unapply, this.DataElementType.unapply)
   
    type MessageName = | MessageName of string
    with
        static member Zero = MessageName ""
        member this.unapply = match this with | MessageName s -> s

    type MessageType = | MessageType of string
    with
        static member Zero = MessageType ""
        member this.unapply = match this with | MessageType s -> s

    type Message = {MessageName : MessageName; MessageType : MessageType option}
    with
        static member Zero = {MessageName = MessageName.Zero; MessageType = None }
        static member apply(_name, _type) = {MessageName = _name; MessageType = _type}
        member this.unapply = (this.MessageName.unapply, this.MessageType |> Option.map (fun t -> t.unapply))

    type MessageBlock = {Messages : List<Message> }
    with 
        static member Zero = {Messages = []}
        static member apply(messages) = {Messages = messages}

    type StateName = | StateName of string
    with
        static member Zero = StateName ""
        member this.unapply = match this with | StateName s -> s

    type StateTransition = | StateTransition of MessageName * List<StateName>
    with 
        member this.unapply = match this with | StateTransition (fst, snd) -> (fst.unapply, snd |> List.map (fun sn -> sn.unapply))

    type StateDefinition = {StateName : StateName; StateTransitions : List<StateTransition>}
    with
        static member Zero = { StateName = StateName.Zero; StateTransitions = [] }
        static member apply(_name, _transitions) = {StateName = _name; StateTransitions = _transitions}

    type MachineName = | MachineName of string
    with
        static member Zero = MachineName ""
        member this.unapply = match this with | MachineName s -> s

    type MachineItem = 
    | InitialState      of StateName
    | Identifier        of Identifier
    | DataBlock         of DataBlock
    | MessageBlock      of MessageBlock
    | StateDefinition   of StateDefinition

    type ParsedMachine = {ParsedMachineName : MachineName; ParsedMachineItems : List<MachineItem> }
    with
        static member Zero = { ParsedMachineName = MachineName.Zero; ParsedMachineItems = [] }
        static member apply(_name, _items) = {ParsedMachineName = _name; ParsedMachineItems = _items }


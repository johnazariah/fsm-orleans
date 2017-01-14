namespace Parser.FSM.Orleans
open FParsec
open System

[<AutoOpen>]
module Parser = 
    // TODO: red squigglies

    let ws p = spaces >>. p .>> spaces

    // http://stackoverflow.com/questions/4047308/keeping-partially-applied-function-generic
    let wstr : string -> Parser<string, unit> = pstring >> ws >> attempt

    let singleTag tagName tagParser = (wstr tagName >>. tagParser)

    let braced p = wstr "{" >>. p .>> wstr "}"
    let private bm m p = (p |> m |> braced)
    let bracedMany  p = bm many  p 
    let bracedMany1 p = bm many1 p 
    let bracedBlock1 blockTag blockItemParser = wstr blockTag >>. (bracedMany1 blockItemParser)
    let bracedNamedBlock blockTag blockNameParser blockItemParser = (wstr blockTag >>. blockNameParser) .>>. bracedMany blockItemParser

    let private ofType typeParser = (wstr "of" >>. typeParser)
    let typedIdentifier nameParser typeParser = nameParser .>>. ofType typeParser
    let optionallyTypedIdentifier nameParser typeParser = nameParser .>>. opt (ofType typeParser)

    let word = (many1Chars asciiLetter) |> ws

    let validMachineName      = word |>> MachineName <?> "Machine Name"
    let validStateName        = word |>> StateName   <?> "State Name"
                             
    let validDataElementName  = word |>> DataElementName <?> "Data Element Name"
    let validDataElementType  = word |>> DataElementType <?> "Data Element Type"
    let validDataElement      = typedIdentifier validDataElementName validDataElementType <?> "Data Element"
                             
    let validMessageName      = word |>> MessageName <?> "Message Name"
    let validMessageType      = word |>> MessageType <?> "Message Type"
    let validMessageElement   = optionallyTypedIdentifier validMessageName validMessageType <?> "Message Element"
                              
    let validIdentifierName   = word |>> IdentifierName <?> "Identifier Name"
    let validIdentifierType   = word |>> IdentifierType <?> "Identifier Type"
    let validIdentifier       = typedIdentifier validIdentifierName validIdentifierType <?> "Valid Identifier"
                              
    let validTargetStates     = sepBy1 validStateName (wstr "|") <?> "Target States"
    let stateTransitionParser = (wstr "on" >>. validMessageName .>> wstr "goto") .>>. validTargetStates |>> StateTransition <?> "State Transition"

    let initialStateParser    = singleTag "initially"  validStateName       |>> InitialState                                                   <?> "Initial State"
    let identifierParser      = singleTag "identifier" validIdentifier      |>> (Identifier.apply >> Identifier)                               <?> "Identifier"
    let dataBlockParser       = singleTag "data"     validDataElement       |>> (DataBlock.apply >> DataBlock)                                 <?> "Data Block"
    let messageBlockParser    = bracedBlock1 "messages" validMessageElement |>> (List.map Message.apply >> MessageBlock.apply >> MessageBlock) <?> "Message Block"
    let stateDefinitionParser = bracedNamedBlock "state" validStateName stateTransitionParser |>> (StateDefinition.apply >> StateDefinition)   <?> "State Definition"
                                                                                                                                       
    let validMachineItem      = identifierParser <|> dataBlockParser <|> messageBlockParser <|> initialStateParser <|> stateDefinitionParser   <?> "Machine Item"
    let machineParser         = bracedNamedBlock "fsm"  validMachineName validMachineItem |>> ParsedMachine.apply                              <?> "Machine"
        
    let parseMachine program =
        match run machineParser program with
        | Success (result, _, _) -> Some result
        | Failure (_, _, _) -> None
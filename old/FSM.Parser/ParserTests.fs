namespace Parser.FSM.Orleans
open FParsec
open System

open AST
open Parser
open BrightSword.CSharpExtensions.DiscriminatedUnion
open BrightSword.RoslynWrapper.Common
open BrightSword.RoslynWrapper.NamespaceDeclaration
open BrightSword.RoslynWrapper.CompilationUnit

[<AutoOpen>]
module ParserTests = 
    let bankAccountStateMachine = """
    fsm BankAccount
    {
	    identifier BankAccountId of Guid

	    data Balance of Balance	    

	    messages
	    {
		    Deposit of Amount
		    Withdrawal of Amount
		    Close
	    }

	    initially ZeroBalance

	    state ZeroBalance 
	    {
		    on Deposit goto Active
		    on Close goto Closed	
	    }

	    state Active
	    {
		    on Deposit goto Active
		    on Withdrawal goto Active | Overdrawn | ZeroBalance
	    }

	    state Overdrawn
	    {
		    on Deposit goto Active | Overdrawn | ZeroBalance
	    }

	    state Closed
	    {
	    }
    }"""

    let private test p str =
        match run p str with
        | Success (result, _, _) -> printfn "Success: %A" result
        | Failure (err, _, _) -> printfn "Failure: %s" err    

    let runTests = 
        test validMachineName "BankAccount"
        test validMessageName "Deposit"
        test validStateName "ZeroBalance"
        test validTargetStates "Active"
        test validTargetStates "Active | Overdrawn | ZeroBalance"

        test identifierParser "identifier BankAccountId of Guid"

        test initialStateParser """initially ZeroBalance"""

        test validDataElement "a of b"
        test dataBlockParser "data Balance of Balance"
        test validMessageElement "Withdrawal of Amount"
        test validMessageElement "Close"
        test messageBlockParser """messages
	    {
		    Deposit of Amount
	    }"""
        test messageBlockParser """messages
	    {
		    Deposit of Amount
		    Withdrawal of Amount
		    Close
	    }"""

        test stateTransitionParser "on Deposit goto Active"
        test stateTransitionParser "on Withdrawal goto Active | Overdrawn | ZeroBalance"        
        test stateTransitionParser """state Closed
	    {
	    }"""
        test stateDefinitionParser """state Overdrawn
        {
		    on Deposit goto Active | Overdrawn | ZeroBalance
	    }"""
        test stateDefinitionParser """state Active
        {
		    on Deposit goto Active
		    on Withdrawal goto Active | Overdrawn | ZeroBalance
	    }"""

        test machineParser bankAccountStateMachine

        bankAccountStateMachine
        |> parseMachine
        |> Option.map(buildValidMachine)
        |> Option.map(printf "%A") |> ignore
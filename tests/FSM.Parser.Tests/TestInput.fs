namespace FSM.Parser.Tests

open FSM.Orleans

[<AutoOpen>]
module TestInput = 
    let C_FSM_BANK_ACCOUNT = """
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

    let BankAccountFSM = text_to_valid_machine C_FSM_BANK_ACCOUNT


using FSM.Orleans;

namespace FSM.BankAccount.Orleans
{
    public class BankAccountGrainState : StateMachineGrainState<BankAccountData, BankAccountState>
    {
        public BankAccountGrainState(BankAccountData stateMachineData, BankAccountState stateMachineState)
            : base(stateMachineData, stateMachineState) {}
    }
}
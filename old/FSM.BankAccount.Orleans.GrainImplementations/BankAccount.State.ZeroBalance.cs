using System.Threading.Tasks;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    { 
        private partial class ZeroBalanceStateMessageHandler
        {
            public async Task<BankAccountGrainState> Deposit(BankAccountGrainState state, Amount amount)
            {
                var newBalance = state.StateMachineData.Match(_ => _.Item.Deposit(amount));

                var stateMachineData = BankAccountData.NewBalance(newBalance);
                var stateMachineState = BankAccountState.ActiveState;

                return await Task.FromResult(new BankAccountGrainState(stateMachineData, stateMachineState));
            }

            public async Task<BankAccountGrainState> Withdraw(BankAccountGrainState state, Amount amount)
            {
                var newBalance = state.StateMachineData.Match(_ => _.Item.Withdraw(amount));

                var stateMachineData = BankAccountData.NewBalance(newBalance);
                var stateMachineState = BankAccountState.OverdrawnState;

                return await Task.FromResult(new BankAccountGrainState(stateMachineData, stateMachineState));
            }

            public async Task<BankAccountGrainState> Close(BankAccountGrainState state)
            {
                var stateMachineData = BankAccountData.NewBalance(Balance.ZeroBalance);
                var stateMachineState = BankAccountState.ClosedState;

                return await Task.FromResult(new BankAccountGrainState(stateMachineData, stateMachineState));
            }
        }
    }
}
using System.Threading.Tasks;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        private partial class ActiveStateMessageHandler
        {
            public Task<ActiveDepositResult> Deposit(BankAccountGrainState state, Amount amount)
            {
                var newBalance = state.StateMachineData.Match(_ => _.Item.Deposit(amount));

                var stateMachineData = BankAccountData.NewBalance(newBalance);
                var stateMachineState = ActiveDepositResultState.ActiveState;

                return Task.FromResult(new ActiveDepositResult(stateMachineData, stateMachineState));
            }

            public Task<ActiveWithdrawResult> Withdraw(BankAccountGrainState state, Amount amount)
            {
                var newBalance = state.StateMachineData.Match(_ => _.Item.Withdraw(amount));

                var stateMachineData = BankAccountData.NewBalance(newBalance);

                var stateMachineState = newBalance.IsZeroBalance
                    ? ActiveWithdrawResultState.ZeroBalanceState
                    : (newBalance.IsOverdrawnBalance
                        ? ActiveWithdrawResultState.OverdrawnState
                        : ActiveWithdrawResultState.ActiveState);

                return Task.FromResult(new ActiveWithdrawResult(stateMachineData, stateMachineState));
            }
        }
    }
}
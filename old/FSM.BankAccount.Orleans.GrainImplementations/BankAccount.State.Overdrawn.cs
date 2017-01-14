using System.Threading.Tasks;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        private partial class OverdrawnStateMessageHandler
        {
            public async Task<BankAccountGrainState> Deposit(BankAccountGrainState state, Amount amount)
            {
                var newBalance = state.StateMachineData.Match(_ => _.Item.Deposit(amount));

                var stateMachineData = BankAccountData.NewBalance(newBalance);
                var stateMachineState = newBalance.IsZeroBalance
                    ? BankAccountState.ZeroBalanceState
                    : newBalance.IsOverdrawnBalance
                        ? BankAccountState.OverdrawnState
                        : BankAccountState.ActiveState;

                return await Task.FromResult(new BankAccountGrainState(stateMachineData, stateMachineState));
            }
        }
    }
}
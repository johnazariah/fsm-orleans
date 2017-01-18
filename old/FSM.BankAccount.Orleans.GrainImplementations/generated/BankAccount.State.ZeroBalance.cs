using System.Threading.Tasks;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        private static async Task<BankAccountGrainState> ZeroBalanceStateProcessor(
            BankAccountGrainState state,
            BankAccountMessage message)
        {
            var messageHandler = new ZeroBalanceStateMessageHandler();
            return
                await
                    message.Match(
                        () => messageHandler.Close(state),
                        _ => messageHandler.Deposit(state, _),
                        _ => messageHandler.Withdraw(state, _));
        }

        private interface IZeroBalanceStateMessageHandler
        {
            Task<BankAccountGrainState> Deposit(BankAccountGrainState state, Amount amount);
            Task<BankAccountGrainState> Withdraw(BankAccountGrainState state, Amount amount);
            Task<BankAccountGrainState> Close(BankAccountGrainState state);
        }

        private partial class ZeroBalanceStateMessageHandler : IZeroBalanceStateMessageHandler {}
    }
}
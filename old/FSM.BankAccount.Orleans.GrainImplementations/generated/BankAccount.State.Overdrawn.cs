using System.Threading.Tasks;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        private static async Task<BankAccountGrainState> OverdrawnStateProcessor(
            BankAccountGrainState state,
            BankAccountMessage message)
        {
            var messageHandler = new OverdrawnStateMessageHandler();

            return
                await
                    message.Match(
                        () => { throw new InvalidMessage(); },
                        _ => messageHandler.Deposit(state, _),
                        _ => { throw new InvalidMessage(); });
        }

        private interface IOverdrawnStateMessageHandler
        {
            Task<BankAccountGrainState> Deposit(BankAccountGrainState state, Amount amount);
        }

        private partial class OverdrawnStateMessageHandler : IOverdrawnStateMessageHandler {}
    }
}
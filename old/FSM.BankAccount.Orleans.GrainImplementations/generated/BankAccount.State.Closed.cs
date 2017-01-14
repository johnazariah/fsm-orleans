using System.Threading.Tasks;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount

    {
        private static Task<BankAccountGrainState> ClosedStateProcessor(
            BankAccountGrainState state,
            BankAccountMessage message)
        {
            throw new InvalidMessage(message);
        }
    }
}
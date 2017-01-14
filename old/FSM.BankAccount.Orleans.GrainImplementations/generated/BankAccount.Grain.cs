using System.Threading.Tasks;
using FSM.BankAccount.Domain;
using FSM.Orleans;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount :
        StateMachineGrain<BankAccountGrainState, BankAccountData, BankAccountState, BankAccountMessage>,
        IBankAccount
    {
        public async Task<BankAccountData> Deposit(Amount amount) => await ProcessMessage(BankAccountMessage.NewDepositMessage(amount));

        public async Task<BankAccountData> Withdrawal(Amount amount)
        {
            return await ProcessMessage(BankAccountMessage.NewWithdrawMessage(amount));
        }

        public async Task<BankAccountData> Close()
        {
            return await ProcessMessage(BankAccountMessage.CloseMessage);
        }
    }
}
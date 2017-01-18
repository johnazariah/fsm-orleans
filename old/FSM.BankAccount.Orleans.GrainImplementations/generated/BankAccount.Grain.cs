using System.Threading.Tasks;
using FSM.BankAccount.Domain;
using FSM.Orleans;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount :
        StateMachineGrain<BankAccountGrainState, BankAccountData, BankAccountState, BankAccountMessage>,
        IBankAccount
    {
        public async Task<BankAccountData> Deposit(Amount amount) => await ProcessMessage(BankAccountMessage.DepositMessage(amount));
        public async Task<BankAccountData> Withdrawal(Amount amount) => await ProcessMessage(BankAccountMessage.WithdrawMessage(amount));
        public async Task<BankAccountData> Close() => await ProcessMessage(BankAccountMessage.CloseMessage);
    }
}
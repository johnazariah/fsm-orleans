using System.Threading.Tasks;
using FSM.BankAccount.Domain;
using FSM.Orleans;

namespace FSM.BankAccount.Orleans
{
    public interface IBankAccount : IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        Task<BankAccountData> Deposit(Amount amount);
        Task<BankAccountData> Withdrawal(Amount amount);
        Task<BankAccountData> Close();
    }
}
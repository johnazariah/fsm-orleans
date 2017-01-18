using System;
using System.Threading.Tasks;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        protected override Func<BankAccountGrainState, BankAccountMessage, Task<BankAccountGrainState>> GetProcessorFunc
            (BankAccountState state) => state.Match<Func<BankAccountGrainState, BankAccountMessage, Task<BankAccountGrainState>>>(
                () => ZeroBalanceStateProcessor,
                () => ActiveStateProcessor,
                () => OverdrawnStateProcessor,
                () => ClosedStateProcessor);
    }
}
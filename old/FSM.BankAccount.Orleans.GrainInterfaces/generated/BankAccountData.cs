using System;

namespace FSM.BankAccount.Orleans
{
    public abstract class BankAccountData
    {
        public static Balance NewBalance(Domain.Balance balance) => new Balance(balance);

        public T Match<T>(Func<Balance, T> balanceFunc)
        {
            return balanceFunc((Balance) this);
        }

        public class Balance : BankAccountData
        {
            public Balance(Domain.Balance balance)
            {
                Item = balance;
            }

            public Domain.Balance Item { get; }
        }
    }
}
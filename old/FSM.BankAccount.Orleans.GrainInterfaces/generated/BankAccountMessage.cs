using System;
using FSM.BankAccount.Domain;

namespace FSM.BankAccount.Orleans
{
    public abstract class BankAccountMessage
    {
        public static readonly BankAccountMessage CloseMessage = new ChoiceTypes.CloseMessage();

        public static BankAccountMessage NewDepositMessage(Amount amount)
            => new ChoiceTypes.DepositMessage(amount);

        public static BankAccountMessage NewWithdrawMessage(Amount amount)
            => new ChoiceTypes.WithdrawMessage(amount);

        public abstract T Match<T>(
            Func<T> closeMessageFunc,
            Func<Amount, T> depositMessageFunc,
            Func<Amount, T> withdrawMessageFunc);


        private static class ChoiceTypes
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            public class CloseMessage : BankAccountMessage
            {
                public override T Match<T>(
                    Func<T> closeMessageFunc,
                    Func<Amount, T> depositMessageFunc,
                    Func<Amount, T> withdrawMessageFunc) => closeMessageFunc();
            }

            public class DepositMessage : BankAccountMessage
            {
                public DepositMessage(Amount item)
                {
                    Item = item;
                }

                private Amount Item { get; }

                public override T Match<T>(
                    Func<T> closeMessageFunc,
                    Func<Amount, T> depositMessageFunc,
                    Func<Amount, T> withdrawMessageFunc)
                {
                    return depositMessageFunc(Item);
                }
            }

            public class WithdrawMessage : BankAccountMessage
            {
                public WithdrawMessage(Amount item)
                {
                    Item = item;
                }

                private Amount Item { get; }

                public override T Match<T>(
                    Func<T> closeMessageFunc,
                    Func<Amount, T> depositMessageFunc,
                    Func<Amount, T> withdrawMessageFunc)
                {
                    return withdrawMessageFunc(Item);
                }
            }

            // ReSharper restore MemberHidesStaticFromOuterClass
        }
    }
}
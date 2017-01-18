using System;

namespace FSM.BankAccount.Orleans
{
    public abstract class BankAccountState
    {
        public static readonly BankAccountState ZeroBalanceState = new ChoiceTypes.ZeroBalanceState();
        public static readonly BankAccountState ActiveState = new ChoiceTypes.ActiveState();
        public static readonly BankAccountState OverdrawnState = new ChoiceTypes.OverdrawnState();
        public static readonly BankAccountState ClosedState = new ChoiceTypes.ClosedState();


        public abstract T Match<T>(
            Func<T> zeroBalanceStateFunc,
            Func<T> activeStateFunc,
            Func<T> overdrawnStateFunc,
            Func<T> closedStateFunc);

        private static class ChoiceTypes
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            public class ZeroBalanceState : BankAccountState
            {
                public override T Match<T>(
                    Func<T> zeroBalanceStateFunc,
                    Func<T> activeStateFunc,
                    Func<T> overdrawnStateFunc,
                    Func<T> closedStateFunc) => zeroBalanceStateFunc();
            }

            public class ActiveState : BankAccountState
            {
                public override T Match<T>(
                    Func<T> zeroBalanceStateFunc,
                    Func<T> activeStateFunc,
                    Func<T> overdrawnStateFunc,
                    Func<T> closedStateFunc) => activeStateFunc();
            }

            public class OverdrawnState : BankAccountState
            {
                public override T Match<T>(
                    Func<T> zeroBalanceStateFunc,
                    Func<T> activeStateFunc,
                    Func<T> overdrawnStateFunc,
                    Func<T> closedStateFunc) => overdrawnStateFunc();
            }

            public class ClosedState : BankAccountState
            {
                public override T Match<T>(
                    Func<T> zeroBalanceStateFunc,
                    Func<T> activeStateFunc,
                    Func<T> overdrawnStateFunc,
                    Func<T> closedStateFunc) => closedStateFunc();
            }

            // ReSharper restore MemberHidesStaticFromOuterClass
        }
    }
}
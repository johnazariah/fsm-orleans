using System;
using System.Threading.Tasks;
using FSM.BankAccount.Domain;
using FSM.Orleans;

namespace FSM.BankAccount.Orleans
{
    public partial class BankAccount
    {
        private static async Task<BankAccountGrainState> ActiveStateProcessor(
            BankAccountGrainState state,
            BankAccountMessage message)
            =>
                await
                    message.Match(
                        HandleInvalidMessage,
                        ActiveStateMessageDelegator.HandleActiveStateDepositMessage(state),
                        ActiveStateMessageDelegator.HandleActiveStateWithdrawMessage(state));

        private static class ActiveStateMessageDelegator
        {
            private static readonly IActiveStateMessageHandler _handler = new ActiveStateMessageHandler();

            public static Func<Amount, Task<BankAccountGrainState>> HandleActiveStateDepositMessage(BankAccountGrainState state) => 
                async _ =>
                {
                    var result = await _handler.Deposit(state, _);
                    return (BankAccountGrainState)(result);
                };

            public static Func<Amount, Task<BankAccountGrainState>> HandleActiveStateWithdrawMessage(BankAccountGrainState state) =>
                async _ =>
                {
                    var result = await _handler.Withdraw(state, _);
                    return (BankAccountGrainState)(result);
                };
        }

        private interface IActiveStateMessageHandler
        {
            Task<ActiveStateMessageHandler.ActiveDepositResult> Deposit(BankAccountGrainState state, Amount amount);
            Task<ActiveStateMessageHandler.ActiveWithdrawResult> Withdraw(BankAccountGrainState state, Amount amount);
        }

        private partial class ActiveStateMessageHandler : IActiveStateMessageHandler
        {
            internal abstract class ActiveDepositResultState
            {
                public static readonly ActiveDepositResultState ActiveState = new ChoiceTypes.ActiveState();
                private readonly BankAccountState _bankAccountState;

                private ActiveDepositResultState(BankAccountState bankAccountState)
                {
                    _bankAccountState = bankAccountState;
                }

                public static explicit operator BankAccountState(ActiveDepositResultState _) => _._bankAccountState;

                private static class ChoiceTypes
                {
                    // ReSharper disable MemberHidesStaticFromOuterClass
                    public class ActiveState : ActiveDepositResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState) { }
                    }

                    // ReSharper restore MemberHidesStaticFromOuterClass
                }
            }

            public class ActiveDepositResult :
                StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult
                    <ActiveDepositResultState>
            {
                public ActiveDepositResult(BankAccountData stateMachineData, ActiveDepositResultState stateMachineState)
                    : base(stateMachineData, stateMachineState) {}

                public static explicit operator BankAccountGrainState(ActiveDepositResult result)
                    => new BankAccountGrainState(result.StateMachineData, (BankAccountState) result.StateMachineState);
            }

            internal abstract class ActiveWithdrawResultState
            {
                public static readonly ActiveWithdrawResultState ActiveState = new ChoiceTypes.ActiveState();
                public static readonly ActiveWithdrawResultState OverdrawnState = new ChoiceTypes.OverdrawnState();
                public static readonly ActiveWithdrawResultState ZeroBalanceState = new ChoiceTypes.ZeroBalanceState();
                private readonly BankAccountState _bankAccountState;

                private ActiveWithdrawResultState(BankAccountState bankAccountState)
                {
                    _bankAccountState = bankAccountState;
                }

                public static explicit operator BankAccountState(ActiveWithdrawResultState _) => _._bankAccountState;

                private static class ChoiceTypes
                {
                    // ReSharper disable MemberHidesStaticFromOuterClass
                    public class ActiveState : ActiveWithdrawResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState) { }
                    }

                    public class OverdrawnState : ActiveWithdrawResultState
                    {
                        public OverdrawnState() : base(BankAccountState.OverdrawnState) { }
                    }

                    public class ZeroBalanceState : ActiveWithdrawResultState
                    {
                        public ZeroBalanceState() : base(BankAccountState.ZeroBalanceState) { }
                    }

                    // ReSharper restore MemberHidesStaticFromOuterClass
                }
            }

            public class ActiveWithdrawResult :
                StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult
                    <ActiveWithdrawResultState>
            {
                public ActiveWithdrawResult(
                    BankAccountData stateMachineData,
                    ActiveWithdrawResultState stateMachineState) : base(stateMachineData, stateMachineState) {}

                public static explicit operator BankAccountGrainState(ActiveWithdrawResult result)
                    => new BankAccountGrainState(result.StateMachineData, (BankAccountState) result.StateMachineState);
            }
        }
    }
}
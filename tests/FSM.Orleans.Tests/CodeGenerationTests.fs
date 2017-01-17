namespace FSM.Parser.Tests

open System

open CSharp.UnionTypes
open BrightSword.RoslynWrapper

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open FSM.Orleans

open NUnit.Framework

[<AutoOpen>]
module CodeGenerationTests =
    [<Test>]
    let ``code-gen: grain interface``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public interface IBankAccount : IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        Task<BankAccountData> Deposit(Amount amount);
        Task<BankAccountData> Withdrawal(Amount amount);
        Task<BankAccountData> Close();
    }
}"

        test_codegen_namespace_member BankAccountFSM build_grain_interface expected

    [<Test>]
    let ``code-gen: data union``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public abstract partial class BankAccountData : IEquatable<BankAccountData>, IStructuralEquatable
    {
        private BankAccountData()
        {
        }

        public abstract TResult Match<TResult>(Func<Amount, TResult> balanceFunc);
        public static BankAccountData Balance(Amount value) => new ChoiceTypes.Balance(value);
        private static partial class ChoiceTypes
        {
            public partial class Balance : BankAccountData
            {
                public Balance(Amount value)
                {
                    Value = value;
                }

                private Amount Value
                {
                    get;
                }

                public override TResult Match<TResult>(Func<Amount, TResult> balanceFunc) => balanceFunc(Value);
                public override bool Equals(object other) => other is Balance && Value.Equals(((Balance)other).Value);
                public override int GetHashCode() => GetType().FullName.GetHashCode() ^ (Value?.GetHashCode() ?? ""null"".GetHashCode());
                public override string ToString() => String.Format(""Balance {0}"", Value);
            }
        }

        public bool Equals(BankAccountData other) => Equals(other as object);
        public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
        public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
        public static bool operator ==(BankAccountData left, BankAccountData right) => left?.Equals(right) ?? false;
        public static bool operator !=(BankAccountData left, BankAccountData right) => !(left == right);
    }
}"

        test_codegen_namespace_member BankAccountFSM build_data_union expected

    [<Test>]
    let ``code-gen: state union``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public abstract partial class BankAccountState : IEquatable<BankAccountState>, IStructuralEquatable
    {
        private BankAccountState()
        {
        }

        public abstract TResult Match<TResult>(Func<TResult> closedStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> activeStateFunc, Func<TResult> zeroBalanceStateFunc);
        public static readonly BankAccountState ClosedState = new ChoiceTypes.ClosedState();
        public static readonly BankAccountState OverdrawnState = new ChoiceTypes.OverdrawnState();
        public static readonly BankAccountState ActiveState = new ChoiceTypes.ActiveState();
        public static readonly BankAccountState ZeroBalanceState = new ChoiceTypes.ZeroBalanceState();
        private static partial class ChoiceTypes
        {
            public partial class ClosedState : BankAccountState
            {
                public override TResult Match<TResult>(Func<TResult> closedStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> activeStateFunc, Func<TResult> zeroBalanceStateFunc) => closedStateFunc();
                public override bool Equals(object other) => other is ClosedState;
                public override int GetHashCode() => GetType().FullName.GetHashCode();
                public override string ToString() => ""ClosedState"";
            }

            public partial class OverdrawnState : BankAccountState
            {
                public override TResult Match<TResult>(Func<TResult> closedStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> activeStateFunc, Func<TResult> zeroBalanceStateFunc) => overdrawnStateFunc();
                public override bool Equals(object other) => other is OverdrawnState;
                public override int GetHashCode() => GetType().FullName.GetHashCode();
                public override string ToString() => ""OverdrawnState"";
            }

            public partial class ActiveState : BankAccountState
            {
                public override TResult Match<TResult>(Func<TResult> closedStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> activeStateFunc, Func<TResult> zeroBalanceStateFunc) => activeStateFunc();
                public override bool Equals(object other) => other is ActiveState;
                public override int GetHashCode() => GetType().FullName.GetHashCode();
                public override string ToString() => ""ActiveState"";
            }

            public partial class ZeroBalanceState : BankAccountState
            {
                public override TResult Match<TResult>(Func<TResult> closedStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> activeStateFunc, Func<TResult> zeroBalanceStateFunc) => zeroBalanceStateFunc();
                public override bool Equals(object other) => other is ZeroBalanceState;
                public override int GetHashCode() => GetType().FullName.GetHashCode();
                public override string ToString() => ""ZeroBalanceState"";
            }
        }

        public bool Equals(BankAccountState other) => Equals(other as object);
        public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
        public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
        public static bool operator ==(BankAccountState left, BankAccountState right) => left?.Equals(right) ?? false;
        public static bool operator !=(BankAccountState left, BankAccountState right) => !(left == right);
    }
}"

        test_codegen_namespace_member BankAccountFSM build_state_union expected

    [<Test>]
    let ``code-gen: message union``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public abstract partial class BankAccountMessage : IEquatable<BankAccountMessage>, IStructuralEquatable
    {
        private BankAccountMessage()
        {
        }

        public abstract TResult Match<TResult>(Func<Amount, TResult> depositMessageFunc, Func<Amount, TResult> withdrawalMessageFunc, Func<TResult> closeMessageFunc);
        public static BankAccountMessage DepositMessage(Amount value) => new ChoiceTypes.DepositMessage(value);
        public static BankAccountMessage WithdrawalMessage(Amount value) => new ChoiceTypes.WithdrawalMessage(value);
        public static readonly BankAccountMessage CloseMessage = new ChoiceTypes.CloseMessage();
        private static partial class ChoiceTypes
        {
            public partial class DepositMessage : BankAccountMessage
            {
                public DepositMessage(Amount value)
                {
                    Value = value;
                }

                private Amount Value
                {
                    get;
                }

                public override TResult Match<TResult>(Func<Amount, TResult> depositMessageFunc, Func<Amount, TResult> withdrawalMessageFunc, Func<TResult> closeMessageFunc) => depositMessageFunc(Value);
                public override bool Equals(object other) => other is DepositMessage && Value.Equals(((DepositMessage)other).Value);
                public override int GetHashCode() => GetType().FullName.GetHashCode() ^ (Value?.GetHashCode() ?? ""null"".GetHashCode());
                public override string ToString() => String.Format(""DepositMessage {0}"", Value);
            }

            public partial class WithdrawalMessage : BankAccountMessage
            {
                public WithdrawalMessage(Amount value)
                {
                    Value = value;
                }

                private Amount Value
                {
                    get;
                }

                public override TResult Match<TResult>(Func<Amount, TResult> depositMessageFunc, Func<Amount, TResult> withdrawalMessageFunc, Func<TResult> closeMessageFunc) => withdrawalMessageFunc(Value);
                public override bool Equals(object other) => other is WithdrawalMessage && Value.Equals(((WithdrawalMessage)other).Value);
                public override int GetHashCode() => GetType().FullName.GetHashCode() ^ (Value?.GetHashCode() ?? ""null"".GetHashCode());
                public override string ToString() => String.Format(""WithdrawalMessage {0}"", Value);
            }

            public partial class CloseMessage : BankAccountMessage
            {
                public override TResult Match<TResult>(Func<Amount, TResult> depositMessageFunc, Func<Amount, TResult> withdrawalMessageFunc, Func<TResult> closeMessageFunc) => closeMessageFunc();
                public override bool Equals(object other) => other is CloseMessage;
                public override int GetHashCode() => GetType().FullName.GetHashCode();
                public override string ToString() => ""CloseMessage"";
            }
        }

        public bool Equals(BankAccountMessage other) => Equals(other as object);
        public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
        public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
        public static bool operator ==(BankAccountMessage left, BankAccountMessage right) => left?.Equals(right) ?? false;
        public static bool operator !=(BankAccountMessage left, BankAccountMessage right) => !(left == right);
    }
}"

        test_codegen_namespace_member BankAccountFSM build_message_union expected


    [<Test>]
    let ``code-gen: grain state class``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public class BankAccountGrainState : StateMachineGrainState<BankAccountData, BankAccountState>
    {
        public BankAccountGrainState(BankAccountData stateMachineData, BankAccountState stateMachineState) : base(stateMachineData, stateMachineState)
        {
        }
    }
}"
        test_codegen_namespace_member BankAccountFSM build_grain_state expected

    [<Test>]
    let ``code-gen-implementation: processor map``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public partial class BankAccount : StateMachineGrain<BankAccountGrainState,BankAccountData,BankAccountState,BankAccountMessage>, IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        protected override Func<BankAccountGrainState, BankAccountMessage, Task<BankAccountGrainState>> GetProcessorFunc(BankAccountGrainState state) => state.Match<Func<BankAccountGrainState, BankAccountMessage, Task<BankAccountGrainState>>>(() => ClosedStateProcessor, () => OverdrawnStateProcessor, () => ActiveStateProcessor, () => ZeroBalanceStateProcessor);
    }
}"

        test_codegen_implementation_member BankAccountFSM to_processor_map expected

    [<Test>]
    let ``code-gen-implementation: message endpoints``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public partial class BankAccount : StateMachineGrain<BankAccountGrainState,BankAccountData,BankAccountState,BankAccountMessage>, IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        public async Task<BankAccountData> Deposit(Amount amount) => await ProcessMessage(BankAccountMessage.Deposit(amount));
        public async Task<BankAccountData> Withdrawal(Amount amount) => await ProcessMessage(BankAccountMessage.Withdrawal(amount));
        public async Task<BankAccountData> Close() => await ProcessMessage(BankAccountMessage.Close());
    }
}"

        test_codegen_implementation_member BankAccountFSM to_message_endpoints expected

    [<Test>]
    let ``code-gen-implementation: state processors``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using FSM.Orleans;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public partial class BankAccount : StateMachineGrain<BankAccountGrainState,BankAccountData,BankAccountState,BankAccountMessage>, IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        private static async Task<BankAccountGrainState> ClosedStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(HandleInvalidMessage, HandleInvalidMessage, HandleInvalidMessage);
        private interface IClosedStateMessageHandler
        {
        }

        private static class ClosedStateMessageDelegator
        {
            private static readonly IClosedStateMessageHandler _handler = new ClosedStateMessageHandler();
        }

        private partial class ClosedStateMessageHandler : IClosedStateMessageHandler
        {
        }

        private static async Task<BankAccountGrainState> OverdrawnStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(OverdrawnStateMessageDelegator.HandleOverdrawnStateDepositMessage(state), HandleInvalidMessage, HandleInvalidMessage);
        private interface IOverdrawnStateMessageHandler
        {
            Task<OverdrawnStateMessageHandler.OverdrawnDepositResult> Deposit(BankAccountGrainState state, Amount amount);
        }

        private static class OverdrawnStateMessageDelegator
        {
            private static readonly IOverdrawnStateMessageHandler _handler = new OverdrawnStateMessageHandler();
            public static Func<Amount,Task<BankAccountGrainState>> HandleOverdrawnStateDepositMessage(BankAccountGrainState state) => (state, amount) => _handler.Deposit(state, amount).ContinueWith(result => (BankAccountGrainState)(result.Result));
        }

        private partial class OverdrawnStateMessageHandler : IOverdrawnStateMessageHandler
        {
            public abstract partial class OverdrawnDepositResultState : IEquatable<OverdrawnDepositResultState>, IStructuralEquatable
            {
                private readonly BankAccountState _base;
                private OverdrawnDepositResultState(BankAccountState value)
                {
                    _base = value;
                }

                public abstract TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc);
                public static readonly OverdrawnDepositResultState ActiveState = new ChoiceTypes.ActiveState();
                public static readonly OverdrawnDepositResultState OverdrawnState = new ChoiceTypes.OverdrawnState();
                public static readonly OverdrawnDepositResultState ZeroBalanceState = new ChoiceTypes.ZeroBalanceState();
                private static partial class ChoiceTypes
                {
                    public partial class ActiveState : OverdrawnDepositResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => activeStateFunc();
                        public override bool Equals(object other) => other is ActiveState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ActiveState"";
                    }

                    public partial class OverdrawnState : OverdrawnDepositResultState
                    {
                        public OverdrawnState() : base(BankAccountState.OverdrawnState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => overdrawnStateFunc();
                        public override bool Equals(object other) => other is OverdrawnState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""OverdrawnState"";
                    }

                    public partial class ZeroBalanceState : OverdrawnDepositResultState
                    {
                        public ZeroBalanceState() : base(BankAccountState.ZeroBalanceState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => zeroBalanceStateFunc();
                        public override bool Equals(object other) => other is ZeroBalanceState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ZeroBalanceState"";
                    }
                }

                public bool Equals(OverdrawnDepositResultState other) => Equals(other as object);
                public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
                public static bool operator ==(OverdrawnDepositResultState left, OverdrawnDepositResultState right) => left?.Equals(right) ?? false;
                public static bool operator !=(OverdrawnDepositResultState left, OverdrawnDepositResultState right) => !(left == right);
                public static explicit operator BankAccountState(OverdrawnDepositResultState value) => value._base;
            }

            public class OverdrawnDepositResult : StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult<OverdrawnDepositResultState>
            {
                public OverdrawnDepositResult(BankAccountData stateMachineData, OverdrawnDepositResultState stateMachineState) : base(stateMachineData, stateMachineState)
                {
                }

                public static explicit operator BankAccountGrainState(OverdrawnDepositResult value) => new BankAccountGrainState(result.StateMachineData, (BankAccountState)result.StateMachineState);
            }
        }

        private static async Task<BankAccountGrainState> ActiveStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(ActiveStateMessageDelegator.HandleActiveStateDepositMessage(state), ActiveStateMessageDelegator.HandleActiveStateWithdrawalMessage(state), HandleInvalidMessage);
        private interface IActiveStateMessageHandler
        {
            Task<ActiveStateMessageHandler.ActiveDepositResult> Deposit(BankAccountGrainState state, Amount amount);
            Task<ActiveStateMessageHandler.ActiveWithdrawalResult> Withdrawal(BankAccountGrainState state, Amount amount);
        }

        private static class ActiveStateMessageDelegator
        {
            private static readonly IActiveStateMessageHandler _handler = new ActiveStateMessageHandler();
            public static Func<Amount,Task<BankAccountGrainState>> HandleActiveStateDepositMessage(BankAccountGrainState state) => (state, amount) => _handler.Deposit(state, amount).ContinueWith(result => (BankAccountGrainState)(result.Result));
            public static Func<Amount,Task<BankAccountGrainState>> HandleActiveStateWithdrawalMessage(BankAccountGrainState state) => (state, amount) => _handler.Withdrawal(state, amount).ContinueWith(result => (BankAccountGrainState)(result.Result));
        }

        private partial class ActiveStateMessageHandler : IActiveStateMessageHandler
        {
            public abstract partial class ActiveDepositResultState : IEquatable<ActiveDepositResultState>, IStructuralEquatable
            {
                private readonly BankAccountState _base;
                private ActiveDepositResultState(BankAccountState value)
                {
                    _base = value;
                }

                public abstract TResult Match<TResult>(Func<TResult> activeStateFunc);
                public static readonly ActiveDepositResultState ActiveState = new ChoiceTypes.ActiveState();
                private static partial class ChoiceTypes
                {
                    public partial class ActiveState : ActiveDepositResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc) => activeStateFunc();
                        public override bool Equals(object other) => other is ActiveState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ActiveState"";
                    }
                }

                public bool Equals(ActiveDepositResultState other) => Equals(other as object);
                public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
                public static bool operator ==(ActiveDepositResultState left, ActiveDepositResultState right) => left?.Equals(right) ?? false;
                public static bool operator !=(ActiveDepositResultState left, ActiveDepositResultState right) => !(left == right);
                public static explicit operator BankAccountState(ActiveDepositResultState value) => value._base;
            }

            public abstract partial class ActiveWithdrawalResultState : IEquatable<ActiveWithdrawalResultState>, IStructuralEquatable
            {
                private readonly BankAccountState _base;
                private ActiveWithdrawalResultState(BankAccountState value)
                {
                    _base = value;
                }

                public abstract TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc);
                public static readonly ActiveWithdrawalResultState ActiveState = new ChoiceTypes.ActiveState();
                public static readonly ActiveWithdrawalResultState OverdrawnState = new ChoiceTypes.OverdrawnState();
                public static readonly ActiveWithdrawalResultState ZeroBalanceState = new ChoiceTypes.ZeroBalanceState();
                private static partial class ChoiceTypes
                {
                    public partial class ActiveState : ActiveWithdrawalResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => activeStateFunc();
                        public override bool Equals(object other) => other is ActiveState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ActiveState"";
                    }

                    public partial class OverdrawnState : ActiveWithdrawalResultState
                    {
                        public OverdrawnState() : base(BankAccountState.OverdrawnState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => overdrawnStateFunc();
                        public override bool Equals(object other) => other is OverdrawnState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""OverdrawnState"";
                    }

                    public partial class ZeroBalanceState : ActiveWithdrawalResultState
                    {
                        public ZeroBalanceState() : base(BankAccountState.ZeroBalanceState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc, Func<TResult> overdrawnStateFunc, Func<TResult> zeroBalanceStateFunc) => zeroBalanceStateFunc();
                        public override bool Equals(object other) => other is ZeroBalanceState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ZeroBalanceState"";
                    }
                }

                public bool Equals(ActiveWithdrawalResultState other) => Equals(other as object);
                public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
                public static bool operator ==(ActiveWithdrawalResultState left, ActiveWithdrawalResultState right) => left?.Equals(right) ?? false;
                public static bool operator !=(ActiveWithdrawalResultState left, ActiveWithdrawalResultState right) => !(left == right);
                public static explicit operator BankAccountState(ActiveWithdrawalResultState value) => value._base;
            }

            public class ActiveDepositResult : StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult<ActiveDepositResultState>
            {
                public ActiveDepositResult(BankAccountData stateMachineData, ActiveDepositResultState stateMachineState) : base(stateMachineData, stateMachineState)
                {
                }

                public static explicit operator BankAccountGrainState(ActiveDepositResult value) => new BankAccountGrainState(result.StateMachineData, (BankAccountState)result.StateMachineState);
            }

            public class ActiveWithdrawalResult : StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult<ActiveWithdrawalResultState>
            {
                public ActiveWithdrawalResult(BankAccountData stateMachineData, ActiveWithdrawalResultState stateMachineState) : base(stateMachineData, stateMachineState)
                {
                }

                public static explicit operator BankAccountGrainState(ActiveWithdrawalResult value) => new BankAccountGrainState(result.StateMachineData, (BankAccountState)result.StateMachineState);
            }
        }

        private static async Task<BankAccountGrainState> ZeroBalanceStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(ZeroBalanceStateMessageDelegator.HandleZeroBalanceStateDepositMessage(state), HandleInvalidMessage, ZeroBalanceStateMessageDelegator.HandleZeroBalanceStateCloseMessage(state));
        private interface IZeroBalanceStateMessageHandler
        {
            Task<ZeroBalanceStateMessageHandler.ZeroBalanceDepositResult> Deposit(BankAccountGrainState state, Amount amount);
            Task<ZeroBalanceStateMessageHandler.ZeroBalanceCloseResult> Close(BankAccountGrainState state);
        }

        private static class ZeroBalanceStateMessageDelegator
        {
            private static readonly IZeroBalanceStateMessageHandler _handler = new ZeroBalanceStateMessageHandler();
            public static Func<Amount,Task<BankAccountGrainState>> HandleZeroBalanceStateDepositMessage(BankAccountGrainState state) => (state, amount) => _handler.Deposit(state, amount).ContinueWith(result => (BankAccountGrainState)(result.Result));
            public static Func<Task<BankAccountGrainState>> HandleZeroBalanceStateCloseMessage(BankAccountGrainState state) => (state) => _handler.Close(state).ContinueWith(result => (BankAccountGrainState)(result.Result));
        }

        private partial class ZeroBalanceStateMessageHandler : IZeroBalanceStateMessageHandler
        {
            public abstract partial class ZeroBalanceDepositResultState : IEquatable<ZeroBalanceDepositResultState>, IStructuralEquatable
            {
                private readonly BankAccountState _base;
                private ZeroBalanceDepositResultState(BankAccountState value)
                {
                    _base = value;
                }

                public abstract TResult Match<TResult>(Func<TResult> activeStateFunc);
                public static readonly ZeroBalanceDepositResultState ActiveState = new ChoiceTypes.ActiveState();
                private static partial class ChoiceTypes
                {
                    public partial class ActiveState : ZeroBalanceDepositResultState
                    {
                        public ActiveState() : base(BankAccountState.ActiveState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> activeStateFunc) => activeStateFunc();
                        public override bool Equals(object other) => other is ActiveState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ActiveState"";
                    }
                }

                public bool Equals(ZeroBalanceDepositResultState other) => Equals(other as object);
                public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
                public static bool operator ==(ZeroBalanceDepositResultState left, ZeroBalanceDepositResultState right) => left?.Equals(right) ?? false;
                public static bool operator !=(ZeroBalanceDepositResultState left, ZeroBalanceDepositResultState right) => !(left == right);
                public static explicit operator BankAccountState(ZeroBalanceDepositResultState value) => value._base;
            }

            public abstract partial class ZeroBalanceCloseResultState : IEquatable<ZeroBalanceCloseResultState>, IStructuralEquatable
            {
                private readonly BankAccountState _base;
                private ZeroBalanceCloseResultState(BankAccountState value)
                {
                    _base = value;
                }

                public abstract TResult Match<TResult>(Func<TResult> closedStateFunc);
                public static readonly ZeroBalanceCloseResultState ClosedState = new ChoiceTypes.ClosedState();
                private static partial class ChoiceTypes
                {
                    public partial class ClosedState : ZeroBalanceCloseResultState
                    {
                        public ClosedState() : base(BankAccountState.ClosedState)
                        {
                        }

                        public override TResult Match<TResult>(Func<TResult> closedStateFunc) => closedStateFunc();
                        public override bool Equals(object other) => other is ClosedState;
                        public override int GetHashCode() => GetType().FullName.GetHashCode();
                        public override string ToString() => ""ClosedState"";
                    }
                }

                public bool Equals(ZeroBalanceCloseResultState other) => Equals(other as object);
                public bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public int GetHashCode(IEqualityComparer comparer) => GetHashCode();
                public static bool operator ==(ZeroBalanceCloseResultState left, ZeroBalanceCloseResultState right) => left?.Equals(right) ?? false;
                public static bool operator !=(ZeroBalanceCloseResultState left, ZeroBalanceCloseResultState right) => !(left == right);
                public static explicit operator BankAccountState(ZeroBalanceCloseResultState value) => value._base;
            }

            public class ZeroBalanceDepositResult : StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult<ZeroBalanceDepositResultState>
            {
                public ZeroBalanceDepositResult(BankAccountData stateMachineData, ZeroBalanceDepositResultState stateMachineState) : base(stateMachineData, stateMachineState)
                {
                }

                public static explicit operator BankAccountGrainState(ZeroBalanceDepositResult value) => new BankAccountGrainState(result.StateMachineData, (BankAccountState)result.StateMachineState);
            }

            public class ZeroBalanceCloseResult : StateMachineGrainState<BankAccountData, BankAccountState>.StateTransitionResult<ZeroBalanceCloseResultState>
            {
                public ZeroBalanceCloseResult(BankAccountData stateMachineData, ZeroBalanceCloseResultState stateMachineState) : base(stateMachineData, stateMachineState)
                {
                }

                public static explicit operator BankAccountGrainState(ZeroBalanceCloseResult value) => new BankAccountGrainState(result.StateMachineData, (BankAccountState)result.StateMachineState);
            }
        }
    }
}"

        test_codegen_implementation_member BankAccountFSM to_state_processors expected

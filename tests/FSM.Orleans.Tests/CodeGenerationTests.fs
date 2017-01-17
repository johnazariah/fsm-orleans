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
    using System;

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
    using System;

    public abstract partial class BankAccountData : IEquatable<BankAccountData>, IStructuralEquatable
    {
        private BankAccountData()
        {
        }

        public abstract TResult Match<TResult>(Func<Balance, TResult> balanceFunc);
        public static BankAccountData Balance(Balance value) => new ChoiceTypes.Balance(value);
        private static partial class ChoiceTypes
        {
            public partial class Balance : BankAccountData
            {
                public Balance(Balance value)
                {
                    Value = value;
                }

                private Balance Value
                {
                    get;
                }

                public override TResult Match<TResult>(Func<Balance, TResult> balanceFunc) => balanceFunc(Value);
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
    using System;

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
    using System;

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
    using System;

    public class BankAccountGrainState : StateMachineGrainState<BankAccountData, BankAccountState>
    {
        public BankAccount(BankAccountData stateMachineData, BankAccountState stateMachineState) : base(stateMachineData, stateMachineState)
        {
        }
    }
}"
        test_codegen_namespace_member BankAccountFSM build_grain_state expected

    [<Test>]
    let ``code-gen-implementation: processor map``() =

        let expected = @"namespace FSM.BankAccount.Orleans
{
    using System;

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
    using System;

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
    using System;

    public partial class BankAccount : StateMachineGrain<BankAccountGrainState,BankAccountData,BankAccountState,BankAccountMessage>, IStateMachineGrain<BankAccountData, BankAccountMessage>
    {
        private static async Task<BankAccountGrainState> ClosedStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(HandleInvalidMessage, HandleInvalidMessage, HandleInvalidMessage);
        private static async Task<BankAccountGrainState> OverdrawnStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(OverdrawnStateMessageDelegator.HandleOverdrawnStateDepositMessage(state), HandleInvalidMessage, HandleInvalidMessage);
        private static async Task<BankAccountGrainState> ActiveStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(ActiveStateMessageDelegator.HandleActiveStateDepositMessage(state), ActiveStateMessageDelegator.HandleActiveStateWithdrawalMessage(state), HandleInvalidMessage);
        private static async Task<BankAccountGrainState> ZeroBalanceStateProcessor(BankAccountGrainState state, BankAccountMessage message) => await message.Match(ZeroBalanceStateMessageDelegator.HandleZeroBalanceStateDepositMessage(state), HandleInvalidMessage, ZeroBalanceStateMessageDelegator.HandleZeroBalanceStateCloseMessage(state));
    }
}"

        test_codegen_implementation_member BankAccountFSM to_state_processors expected

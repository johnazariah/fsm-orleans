using System.Diagnostics.CodeAnalysis;
using FSM.BankAccount.Domain;
using Xunit;

namespace FSM.BankAccount.Tests
{
    [ExcludeFromCodeCoverage]
    public class BalanceTests
    {
        private static readonly Balance ZeroBalance = Balance.ZeroBalance;
        private static readonly Amount SomeAmount = new Amount(10.0M);
        private static readonly Amount SmallerAmount = new Amount(1.0M);
        private static readonly Amount BiggerAmount = new Amount(100.0M);
        private static readonly Balance ActiveBalance = Balance.NewActiveBalance(SomeAmount);
        private static readonly Balance OverdrawnBalance = Balance.NewOverdrawnBalance(SomeAmount);

        [Fact]
        public void CanCreateZeroBalance()
        {
            Assert.NotNull(ZeroBalance);
            Assert.IsAssignableFrom<Balance>(ZeroBalance);
        }

        [Fact]
        public void DepositingAnAmountToAZeroBalanceMakesAnActiveBalance()
        {
            var result = ZeroBalance.Deposit(SomeAmount);

            Assert.IsType<Balance.ActiveBalance>(result);
            Assert.Equal(SomeAmount, ((Balance.ActiveBalance) result).Item);
        }

        [Fact]
        public void WithdrawingAnAmountFromAZeroBalanceMakesAnOverdrawnBalance()
        {
            var result = ZeroBalance.Withdraw(SomeAmount);

            Assert.IsType<Balance.OverdrawnBalance>(result);
            Assert.Equal(SomeAmount, ((Balance.OverdrawnBalance) result).Item);
        }

        [Fact]
        public void DepositingAndWithdrawingTheSameAmountFromAZeroBalanceMakesAZeroBalance()
        {
            var result = ZeroBalance.Deposit(SomeAmount).Withdraw(SomeAmount);
            Assert.Equal(Balance.ZeroBalance, result);
        }

        [Fact]
        public void WithdrawingAndDepositingTheSameAmountFromAZeroBalanceMakesAZeroBalance()
        {
            var result = ZeroBalance.Withdraw(SomeAmount).Deposit(SomeAmount);
            Assert.Equal(Balance.ZeroBalance, result);
        }

        [Fact]
        public void DepositingAnAmountTonActiveBalanceLeavesAnActiveBalance()
        {
            var input = ActiveBalance;
            var result = input.Deposit(SmallerAmount);
            Assert.IsType<Balance.ActiveBalance>(result);
            Assert.Equal(SomeAmount + SmallerAmount, ((Balance.ActiveBalance) result).Item);
        }

        [Fact]
        public void WithdrawingAnAmountFromAnOverdrawnBalanceLeavesAnOverdrawnBalance()
        {
            var input = OverdrawnBalance;
            var result = input.Withdraw(SmallerAmount);
            Assert.IsType<Balance.OverdrawnBalance>(result);
            Assert.Equal(SomeAmount + SmallerAmount, ((Balance.OverdrawnBalance) result).Item);
        }

        [Fact]
        public void WithdrawingASmallerAmountFromAnActiveBalanceLeavesAnActiveBalance()
        {
            var input = ActiveBalance;
            var result = input.Withdraw(SmallerAmount);
            Assert.IsType<Balance.ActiveBalance>(result);
            Assert.Equal(SomeAmount - SmallerAmount, ((Balance.ActiveBalance) result).Item);
        }

        [Fact]
        public void DepositingASmallerAmountToAnOverdrawnBalanceLeavesAnOverdrawnBalance()
        {
            var input = OverdrawnBalance;
            var result = input.Deposit(SmallerAmount);
            Assert.IsType<Balance.OverdrawnBalance>(result);
            Assert.Equal(SomeAmount - SmallerAmount, ((Balance.OverdrawnBalance) result).Item);
        }

        [Fact]
        public void WithdrawingABiggerAmountFromAnActiveBalanceLeavesAnOverdrawnBalance()
        {
            var input = ActiveBalance;
            var result = input.Withdraw(BiggerAmount);
            Assert.IsType<Balance.OverdrawnBalance>(result);
            Assert.Equal(BiggerAmount - SomeAmount, ((Balance.OverdrawnBalance) result).Item);
        }

        [Fact]
        public void DepositingABiggerAmountToAnOverdrawnBalanceLeavesAnActiveBalance()
        {
            var input = OverdrawnBalance;
            var result = input.Deposit(BiggerAmount);
            Assert.IsType<Balance.ActiveBalance>(result);
            Assert.Equal(BiggerAmount - SomeAmount, ((Balance.ActiveBalance) result).Item);
        }
    }
}
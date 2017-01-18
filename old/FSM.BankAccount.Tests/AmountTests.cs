using System;
using System.Diagnostics.CodeAnalysis;
using FSM.BankAccount.Domain;
using Xunit;

namespace FSM.BankAccount.Tests
{
    [ExcludeFromCodeCoverage]
    public class AmountTests
    {
        [Fact]
        public void CreatingAmountWithPositiveDecimalSucceeds()
        {
            Assert.IsType<Amount>(new Amount(10.0M));
        }

        [Fact]
        public void CreatingAmountWithZeroSucceeds()
        {
            Assert.IsType<Amount>(new Amount(0.0M));
        }

        [Fact]
        public void CreatingAmountWithNegativeDecimalThrows()
        {
            Assert.Throws<Exception>(() => new Amount(-10.0M));
        }

        [Fact]
        public void AccessingTheValuePropertySucceeds()
        {
            Assert.Equal(10.0M, new Amount(10.0M).Value);
        }

        [Fact]
        public void SimpleAdditionOfAmountsSucceeds()
        {
            Assert.Equal(new Amount(10.0M), (new Amount(5.0M) + new Amount(5.0M)));
        }

        [Fact]
        public void SimpleSubtractionOfAmountsSucceeds()
        {
            Assert.Equal(new Amount(10.0M), (new Amount(15.0M) - new Amount(5.0M)));
        }
    }
}
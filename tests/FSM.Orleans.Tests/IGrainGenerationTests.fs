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
module IGrainGenerationTests = 
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

        test_codegen_interface BankAccountFSM build_grain_interface expected

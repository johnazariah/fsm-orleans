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

        let expected = @"namespace DU.Tests
{
    using System;
    using System.Collections;

    public abstract partial class Maybe<T> : IEquatable<Maybe<T>>, IStructuralEquatable
    {
        private Maybe()
        {
        }
    }
}"

        test_codegen_interface BankAccountFSM build_grain_interface expected

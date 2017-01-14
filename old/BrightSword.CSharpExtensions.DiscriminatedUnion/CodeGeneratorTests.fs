namespace BrightSword.CSharpExtensions.DiscriminatedUnion

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

open BrightSword.RoslynWrapper.Common
open BrightSword.RoslynWrapper.NamespaceDeclaration
open BrightSword.RoslynWrapper.CompilationUnit

type SF = SyntaxFactory

module CodeGeneratorTests =
    let runTests =
        let optionDU = 
            { DiscriminatedUnion.Zero with 
                DiscriminatedUnionName = "Option"
                TypeParameters = ["T"]
                DiscriminatedUnionMembers = [ DiscriminatedUnionMember.apply("None"); DiscriminatedUnionMember.apply("Some", "T")]
            }

        let eitherDU = 
            { DiscriminatedUnion.Zero with 
                DiscriminatedUnionName = "Either"
                TypeParameters = ["T"; "E"]
                DiscriminatedUnionMembers = [ DiscriminatedUnionMember.apply("Result", "T"); DiscriminatedUnionMember.apply("Error", "E")] 
            }

        let classes = [ optionDU; eitherDU ] |> Seq.map (fun c -> CodeGenerator.toClassDeclaration c :> MemberDeclarationSyntax)

        ``compilation unit`` [
            ``namespace`` "CoolMonads"
                ``{`` 
                    []
                    classes
                ``}`` :> MemberDeclarationSyntax
            ]
        |> (BrightSword.RoslynWrapper.CompilationUnit.toCodeString >> printf "%s") |> ignore


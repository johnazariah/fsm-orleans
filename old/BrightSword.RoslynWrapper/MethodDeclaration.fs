namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module MethodDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type SF = SyntaxFactory

    let private setModifiers modifiers (md : MethodDeclarationSyntax) =
        modifiers 
        |> Seq.map SF.Token
        |> SF.TokenList 
        |> md.WithModifiers 

    let private setParameterList methodParams (md : MethodDeclarationSyntax) =
        methodParams 
        |> toParameterList
        |> md.WithParameterList
    
    let private setExpressionBody methodBody (md : MethodDeclarationSyntax) =
        methodBody 
        |> Option.fold (fun (_md : MethodDeclarationSyntax) _mb -> _md.WithExpressionBody _mb) md

    let private setTypeParameters typeParameters (md : MethodDeclarationSyntax) =
        if typeParameters |> Seq.isEmpty then md
        else
        typeParameters 
        |> Seq.map (SF.Identifier >> SF.TypeParameter)
        |> (SF.SeparatedList >> SF.TypeParameterList)
        |> md.WithTypeParameterList

    let private addClosingSemicolon (md : MethodDeclarationSyntax) =
        SyntaxKind.SemicolonToken |> SF.Token 
        |> md.WithSemicolonToken

    let ``method`` methodType methodName ``<<`` methodTypeParameters ``>>`` 
            ``(`` methodParams ``)`` 
            modifiers 
            ``=>`` methodBody =        
        (methodType |> toIdentifierName,  methodName |> SF.Identifier) |> SF.MethodDeclaration
        |> setTypeParameters methodTypeParameters
        |> setModifiers modifiers
        |> setParameterList methodParams
        |> setExpressionBody methodBody
        |> addClosingSemicolon
            
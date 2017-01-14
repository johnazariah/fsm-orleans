namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module PropertyDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type SF = SyntaxFactory
        
    let private setModifiers modifiers (pd : PropertyDeclarationSyntax) =
        modifiers 
        |> Seq.map SF.Token
        |> SF.TokenList 
        |> pd.WithModifiers

    let private setGetAccessor (pd : PropertyDeclarationSyntax) =
        SyntaxKind.GetAccessorDeclaration 
        |> SF.AccessorDeclaration 
        |> (fun ad -> ad.WithSemicolonToken(SyntaxKind.SemicolonToken |> SF.Token))
        |> (fun ad -> pd.AddAccessorListAccessors ad)

    let ``propg`` propertyType propertyName modifiers =
        (propertyType |> toIdentifierName, propertyName |> SF.Identifier)
        |> SF.PropertyDeclaration
        |> setModifiers modifiers
        |> setGetAccessor


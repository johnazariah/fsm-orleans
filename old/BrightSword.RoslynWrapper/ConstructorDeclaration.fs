namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module ConstructorDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory
    
    let private setModifiers modifiers (cd : ConstructorDeclarationSyntax) =
        modifiers 
        |> Seq.map SF.Token
        |> SF.TokenList 
        |> cd.WithModifiers         

    let private setParameterList constructorParams (cd : ConstructorDeclarationSyntax) =
        constructorParams 
        |> Seq.map (fun (paramName, paramType) -> ``param`` paramName ``of`` paramType)
        |> (SF.SeparatedList >> SF.ParameterList)
        |> cd.WithParameterList

    let private setBodyBlock bodyBlockStatements (cd : ConstructorDeclarationSyntax) =
        bodyBlockStatements         
        |> (Seq.toArray >> SF.Block)
        |> cd.WithBody 

    let private setInitializer baseConstructorParameters (cd : ConstructorDeclarationSyntax) =
        if baseConstructorParameters |> Seq.isEmpty then cd else
        baseConstructorParameters
        |> Seq.map (toIdentifierName >> SF.Argument)
        |> (SF.SeparatedList >> SF.ArgumentList)
        |> (fun args -> SF.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, args))
        |> cd.WithInitializer 
    
    let ``constructor`` className ``(`` parameters ``)`` 
            ``:`` baseConstructorParameters
            modifiers
            ``{`` 
                bodyBlockStatements
            ``}`` =
        className 
        |> (SF.Identifier >> SF.ConstructorDeclaration)
        |> setInitializer baseConstructorParameters
        |> setParameterList parameters
        |> setModifiers modifiers
        |> setBodyBlock bodyBlockStatements

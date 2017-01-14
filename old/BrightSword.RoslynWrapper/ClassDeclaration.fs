namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module ClassDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory

    let private setModifiers modifiers (cd : ClassDeclarationSyntax)  = 
        modifiers 
        |> Seq.map SF.Token 
        |> SF.TokenList 
        |> cd.WithModifiers

    let private setMembers members (cd : ClassDeclarationSyntax) =
        cd.AddMembers (members |> Seq.toArray)

    let private setBases bases (cd : ClassDeclarationSyntax) =
        if bases |> Seq.isEmpty then cd else
        bases 
        |> Seq.map (toIdentifierName >> SF.SimpleBaseType >> (fun b -> b :> BaseTypeSyntax))
        |> (SF.SeparatedList >> SF.BaseList)
        |> cd.WithBaseList

    let private setTypeParameters typeParameters (cd : ClassDeclarationSyntax) =
        if typeParameters |> Seq.isEmpty then cd else
        typeParameters 
        |> Seq.map (SF.Identifier >> SF.TypeParameter)
        |> (SF.SeparatedList >> SF.TypeParameterList)
        |> cd.WithTypeParameterList

    let ``class`` className ``<<`` typeParameters ``>>``
            ``:`` baseClassName ``,`` baseInterfaces
            modifiers
            ``{``
                 members
            ``}`` =             
            className |> (SF.Identifier >> SF.ClassDeclaration)
            |> setTypeParameters typeParameters
            |> setBases (baseClassName ?+ baseInterfaces)
            |> setModifiers modifiers
            |> setMembers members

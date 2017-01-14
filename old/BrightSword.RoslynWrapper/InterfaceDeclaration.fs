namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module InterfaceDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory

    let private setModifiers modifiers (cd : InterfaceDeclarationSyntax)  = 
        modifiers 
        |> Seq.map SF.Token 
        |> SF.TokenList 
        |> cd.WithModifiers

    let private setMembers members (cd : InterfaceDeclarationSyntax) =
        cd.AddMembers (members |> Seq.toArray)

    let private setBases bases (cd : InterfaceDeclarationSyntax) =
        if bases |> Seq.isEmpty then cd else
        bases 
        |> Seq.map (toIdentifierName >> SF.SimpleBaseType >> (fun b -> b :> BaseTypeSyntax))
        |> (SF.SeparatedList >> SF.BaseList)
        |> cd.WithBaseList

    let private setTypeParameters typeParameters (cd : InterfaceDeclarationSyntax) =
        if typeParameters |> Seq.isEmpty then cd
        else
        typeParameters 
        |> Seq.map (SF.Identifier >> SF.TypeParameter)
        |> (SF.SeparatedList >> SF.TypeParameterList)
        |> cd.WithTypeParameterList

    let ``interface`` interfaceName ``<<`` typeParameters ``>>``
            ``:`` baseInterfaces
            modifiers
            ``{``
                 members
            ``}`` =             
            interfaceName |> (SF.Identifier >> SF.InterfaceDeclaration)
            |> setTypeParameters typeParameters
            |> setBases baseInterfaces
            |> setModifiers modifiers
            |> setMembers members


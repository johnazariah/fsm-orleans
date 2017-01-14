namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module FieldDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type SF = SyntaxFactory

    let private setVariableInitializer initializer (vd : VariableDeclaratorSyntax) =
        initializer
        |> Option.fold (fun (_vd : VariableDeclaratorSyntax) _in -> _vd.WithInitializer _in) vd

    let private setFieldVariable fieldName fieldInitializer (vd : VariableDeclarationSyntax) = 
        [ fieldName ]
        |> Seq.map (SF.Identifier >> SF.VariableDeclarator >> setVariableInitializer fieldInitializer)
        |> SF.SeparatedList
        |> vd.WithVariables

    let private setModifiers modifiers (fd : FieldDeclarationSyntax)  = 
        modifiers 
        |> Seq.map SF.Token
        |> SF.TokenList 
        |> fd.WithModifiers

    let ``field`` fieldType fieldName modifiers ``=>`` fieldInitializer = 
        fieldType 
        |> (toIdentifierName >> SF.VariableDeclaration)
        |> setFieldVariable fieldName fieldInitializer 
        |> SF.FieldDeclaration
        |> setModifiers modifiers
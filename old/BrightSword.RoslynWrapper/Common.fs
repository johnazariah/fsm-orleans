namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module Common =    
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory
    
    let ``:`` = None
    let ``,`` = None
    let ``}`` = None
    let ``{`` statements ``}`` = statements |> Seq.toArray<StatementSyntax> |> SF.SeparatedList |> SF.Block
    let ``<<`` = None
    let ``>>`` = None
    let ``)`` = None
    let ``(`` expression ``)`` = expression |> SF.ParenthesizedExpression
    
    let ``private`` = SyntaxKind.PrivateKeyword
    let ``protected`` = SyntaxKind.ProtectedKeyword
    let ``internal`` = SyntaxKind.InternalKeyword
    let ``public`` = SyntaxKind.PublicKeyword
    let ``partial`` = SyntaxKind.PartialKeyword
    let ``abstract`` = SyntaxKind.AbstractKeyword    
    let ``async`` = SyntaxKind.AsyncKeyword
    let ``virtual`` = SyntaxKind.VirtualKeyword
    let ``override`` = SyntaxKind.OverrideKeyword
    let ``static`` = SyntaxKind.StaticKeyword
    let ``readonly`` = SyntaxKind.ReadOnlyKeyword

    let ``of`` = None

    let toIdentifierName = SF.Identifier >> SF.IdentifierName
    
    let (?+) option list =
        option |> Option.fold (fun l o -> o :: l) list

    let mapTuple2 f (t1, t2)= (f t1, f t2)
    
    let toQualifiedName parts =
        let rec toQualifiedNameImpl = function
            | [] -> failwith "cannot get qualified name of empty list"
            | [ n ] -> n |> toIdentifierName :> NameSyntax
            | [ p1 ; p2 ] -> (p2, p1) |> mapTuple2 toIdentifierName |> SF.QualifiedName :> NameSyntax
            | p1 :: rest -> (toQualifiedNameImpl rest, p1 |> toIdentifierName) |> SF.QualifiedName :> NameSyntax
        in parts |> List.rev |> toQualifiedNameImpl
 
    let toType = (toIdentifierName >> (fun t -> t :> TypeSyntax))

    let toParameterName (str : string) =
        sprintf "%s%s" (str.Substring(0, 1).ToLower()) (str.Substring(1))
           
    let ``:=`` expression = expression |> SF.EqualsValueClause 
    let ``=>`` expression = expression |> SF.ArrowExpressionClause    
    let ``await`` expression = expression |> SF.AwaitExpression
    let ``return`` expression = expression |> SF.ReturnStatement

    let ``param`` paramName ``of`` paramType =
        paramName 
        |> (SF.Identifier >> SF.Parameter)
        |> (fun p -> paramType |> toIdentifierName |> p.WithType)

    let toParameterList parameters =
        parameters
        |> Seq.map (fun (paramName, paramType) -> ``param`` paramName ``of`` paramType)
        |> (SF.SeparatedList >> SF.ParameterList)
        
    let ``argument`` = (toIdentifierName >> SF.Argument)

    let toArgumentList arguments = 
        arguments |> Seq.map ``argument``

    let (<--) target source =
         SF.AssignmentExpression 
            (SyntaxKind.SimpleAssignmentExpression, 
             target |> SF.Identifier |> SF.IdentifierName, 
             source |> SF.Identifier |> SF.IdentifierName)

    let (<.>) a b = 
        SF.MemberAccessExpression
            (SyntaxKind.SimpleMemberAccessExpression,
             a,
             b)

    let ``() =>`` parameters expression = 
        expression 
        |> SF.ParenthesizedLambdaExpression 
        |> (fun ple -> ple.AddParameterListParameters (parameters |> Seq.map (SF.Identifier >> SF.Parameter) |> Seq.toArray))

    let ``throw`` = SF.ThrowStatement
    let ``_ =>`` parameterName expression = 
        SF.SimpleLambdaExpression (parameterName |> (SF.Identifier >> SF.Parameter), expression)

    let ``cast`` targetType expression = 
        SF.CastExpression (targetType |> toIdentifierName, expression)
[<AutoOpen>]
module GenericName = 
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory

    let private setTypeArgumentList typeArguments (gn : GenericNameSyntax) =
        typeArguments
        |> Seq.map (toIdentifierName >> (fun t -> t :> TypeSyntax))
        |> (SF.SeparatedList >> SF.TypeArgumentList)
        |> gn.WithTypeArgumentList
 
    let ``generic type`` typeName ``<<`` typeArguments ``>>`` =
        typeName
        |> (SF.Identifier >> SF.GenericName)
        |> setTypeArgumentList typeArguments

[<AutoOpen>]
module ObjectCreation = 
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory

    let private setArguments arguments (oce : ObjectCreationExpressionSyntax) = 
        arguments 
        |> Seq.map (toIdentifierName >> SF.Argument) 
        |> (SF.SeparatedList >> SF.ArgumentList)
        |> oce.WithArgumentList
            
    let ``new`` nameparts ``(`` arguments ``)`` = 
        nameparts
        |> toQualifiedName
        |> SF.ObjectCreationExpression
        |> setArguments arguments

[<AutoOpen>]
module Invocation =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory
    
    let private setArguments (methodArguments : ArgumentSyntax seq) (ie : InvocationExpressionSyntax) =
        methodArguments
        |> (SF.SeparatedList >> SF.ArgumentList)
        |> ie.WithArgumentList
        
    let ``invoke`` m ``(`` methodArguments ``)`` =
        m
        |> SF.InvocationExpression
        |> setArguments methodArguments

[<AutoOpen>]
module Conversion =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory
    
    let private setModifiers modifiers (co : ConversionOperatorDeclarationSyntax) =
        modifiers 
        |> Seq.map SF.Token
        |> SF.TokenList 
        |> co.WithModifiers
        
    let private addClosingSemicolon (co : ConversionOperatorDeclarationSyntax) =
        SyntaxKind.SemicolonToken |> SF.Token 
        |> co.WithSemicolonToken

    let ``explicit operator`` target ``(`` source ``)`` ``=>`` initializer =
        (SyntaxKind.ExplicitKeyword |> SF.Token, target |> toIdentifierName)
        |> SF.ConversionOperatorDeclaration
        |> (fun co -> co.WithParameterList <| toParameterList [ ("value", source) ])
        |> setModifiers [``public``; ``static``]
        |> (fun co -> co.WithExpressionBody initializer)
        |> addClosingSemicolon

    let ``implicit operator`` target ``(`` source ``)`` modifiers ``=>`` initializer =
        (SyntaxKind.ImplicitKeyword |> SF.Token, target |> toIdentifierName)
        |> SF.ConversionOperatorDeclaration
        |> (fun co -> co.WithParameterList <| toParameterList [ ("value", source) ])
        |> setModifiers modifiers
        |> (fun co -> co.WithExpressionBody initializer)
        |> addClosingSemicolon
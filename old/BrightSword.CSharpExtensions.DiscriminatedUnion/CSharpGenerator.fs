namespace BrightSword.CSharpExtensions.DiscriminatedUnion

module CodeGenerator =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    open BrightSword.RoslynWrapper.Common
    open BrightSword.RoslynWrapper.FieldDeclaration
    open BrightSword.RoslynWrapper.MethodDeclaration
    open BrightSword.RoslynWrapper.PropertyDeclaration
    open BrightSword.RoslynWrapper.ConstructorDeclaration
    open BrightSword.RoslynWrapper.ClassDeclaration
    open BrightSword.RoslynWrapper.ObjectCreation
    open BrightSword.RoslynWrapper.Conversion

    type SF = SyntaxFactory

    let private toAccessMember duName =
        function
        | (e, []) ->
            let initializer = (:=) (``new`` [ "ChoiceTypes"; e ] ``(`` [] ``)``)
            field duName e [ ``public``; ``static``; readonly ] (:=) (Some initializer) :> MemberDeclarationSyntax
            |>  // BUG
                Some
        | (c, args) ->
            let methodName = c |> sprintf " // BUG
                                            New%s"
            let ctorArguments = args |> Seq.map (fun (a : string) -> a.ToLower())
            let methodParameters = args |> Seq.map (fun (a : string) -> (a.ToLower(), a))
            let expression = (=>) (``new`` [ "ChoiceTypes"; c ] ``(`` ctorArguments ``)``)
            method duName methodName (<<) [] (>>) ``(`` methodParameters ``)`` [ ``public``; ``static`` ] (=>)
                (Some expression) :> MemberDeclarationSyntax |> Some

    let private toFuncParameterName = toParameterName >> sprintf "%sFunc"

    let private toMatchFuncParameter =
        function
        | (e, []) -> (e |> toFuncParameterName, "Func<TResult>")
        | (c, args) ->
            (c |> toFuncParameterName,
             args
             |> Seq.toArray
             |> (fun (rg : System.String array) -> System.String.Join(",", rg))
             |> sprintf "Func<%s, TResult>")

    let private toMatchFuncParameters du =
        du.DiscriminatedUnionMembers |> Seq.map (fun duMember -> toMatchFuncParameter duMember.unapply)

    let private toPrivateConstructor du =
        du.IsSubsetOf
        |> Option.map
               (fun baseType ->
               let baseTypeArg = baseType |> toParameterName
               constructor du.DiscriminatedUnionName ``(`` [ (baseTypeArg, baseType) ] ``)`` ``:`` [] [ ``private`` ]
                   ``{`` [ ("BaseValue" <-- baseTypeArg) |> SF.ExpressionStatement ] ``}`` :> MemberDeclarationSyntax)

    let private toPrivateBaseValueProperty du =
        du.IsSubsetOf |> Option.map (fun b -> propg b "BaseValue" [ ``private`` ] :> MemberDeclarationSyntax)

    let private toMatchFunction typeParamName invocation du =
        let toMatchFuncParameter typeParamName (memberName, memberTypeList) =
            let matchFuncReturnType =
                memberTypeList
                |> List.fold (fun _ m -> sprintf "Func<%s, %s>" m typeParamName) (sprintf "Func<%s>" typeParamName)
            (memberName |> toFuncParameterName, matchFuncReturnType)

        let matchFuncParameters =
            du.DiscriminatedUnionMembers
            |> Seq.map (fun duMember -> toMatchFuncParameter typeParamName duMember.unapply)
        method typeParamName "Match" (<<) [ typeParamName ] (>>) ``(`` matchFuncParameters ``)``
            [ ``public``
              (invocation |> Option.fold (fun _ s -> ``override``) ``abstract``) ] (=>) invocation :> MemberDeclarationSyntax
        |> Some

    let private toMatchFunctionDeclaration = toMatchFunction "TResult" None

    let private toClassName du =
        if du.TypeParameters |> Seq.isEmpty then du.DiscriminatedUnionName
        else sprintf "%s<%s>" du.DiscriminatedUnionName (du.TypeParameters |> String.concat ",")

    let private toChoiceClass du (memberName, argTypeList) =
        let choiceClassConstructor =
            match (du.IsSubsetOf, argTypeList) with
            | None, [] -> None
            | Some baseType, [] ->
                constructor memberName ``(`` [] ``)`` ``:`` [ (sprintf "%s.%s" baseType memberName) ] [ ``public`` ]
                    ``{`` [] ``}`` :> MemberDeclarationSyntax |> Some
            | None, args ->
                let ctorParameters = args |> Seq.map (fun (a : string) -> (a.ToLower(), a))
                let memberAssignments =
                    args
                    |> Seq.map (fun (a : string) -> (a <-- a.ToLower()) |> SF.ExpressionStatement :> StatementSyntax)
                constructor memberName ``(`` ctorParameters ``)`` ``:`` [] [ ``public`` ] ``{`` memberAssignments ``}`` :> MemberDeclarationSyntax
                |> Some
            | Some _, _ -> failwith "cannot extend a DU with constructor members"

        let choiceClassItemProperty =
            argTypeList
            |> Seq.map (fun argType -> propg argType argType [ ``private`` ] :> MemberDeclarationSyntax |> Some)

        let choiceClassMatchFunctionOverride =
            let invocation =
                let invocationParameterFuncArgumentList =
                    argTypeList
                    |> List.fold (fun _ s -> ([ s |> (SF.IdentifierName >> SF.Argument) ])) []
                    |> (SF.SeparatedList >> SF.ArgumentList)
                memberName
                |> toParameterName
                |> (toIdentifierName >> SF.InvocationExpression)
                |> (fun ie -> ie.WithArgumentList invocationParameterFuncArgumentList)
                |> SF.ArrowExpressionClause
            du |> toMatchFunction "TResult" (Some invocation)

        let members =
            seq {
                yield choiceClassConstructor
                yield! choiceClassItemProperty
                yield choiceClassMatchFunctionOverride
            }

        ``class`` memberName (<<) [] (>>) ``:`` (Some(du |> toClassName)) ``,`` [] [ ``public`` ] ``{``
            (members |> Seq.choose (id)) ``}`` :> MemberDeclarationSyntax

    let toBaseCastOperator du =
        du.IsSubsetOf
        |> Option.map
               (fun baseType ->
               let value = "value" |> toIdentifierName
               let baseValue = "BaseValue" |> toIdentifierName
               let initializer = (=>) (value <.> baseValue)
               ``explicit operator`` baseType ``(`` du.DiscriminatedUnionName ``)`` (=>) initializer :> MemberDeclarationSyntax)

    let private toChoiceTypeWrapperClass du =
        let innerClasses = du.DiscriminatedUnionMembers |> Seq.map (fun duMember -> toChoiceClass du duMember.unapply)
        ``class`` "ChoiceTypes" (<<) [] (>>) ``:`` None ``,`` [] [ ``private``; ``static`` ] ``{`` innerClasses ``}`` :> MemberDeclarationSyntax
        |> Some

    let toClassDeclaration du =
        let members =
            seq {
                yield! du.DiscriminatedUnionMembers
                       |> Seq.map (fun duMember -> toAccessMember (du |> toClassName) duMember.unapply)
                yield du |> toPrivateConstructor
                yield du |> toPrivateBaseValueProperty
                yield du |> toMatchFunctionDeclaration
                yield du |> toBaseCastOperator
                yield du |> toChoiceTypeWrapperClass
            }
        ``class`` du.DiscriminatedUnionName (<<) du.TypeParameters (>>) ``:`` None ``,`` []
            [ ``public``; ``abstract``; partial ] ``{`` (members |> Seq.choose id) ``}``

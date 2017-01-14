namespace BrightSword.CSharpExtensions.DiscriminatedUnion
open FParsec
open System

[<AutoOpen>]
module AST =
    type UnionTypeName = 
    | UnionTypeName of string
    with 
        member this.unapply = match this with | UnionTypeName x -> x

    type TypeArgument = 
    | TypeArgument of string
    with 
        member this.unapply = match this with | TypeArgument x -> x

    type UnionMemberName = 
    | UnionMemberName of string
    with 
        member this.unapply = match this with | UnionMemberName x -> x

    type CaseMember = 
    | CaseClass of UnionMemberName * TypeArgument list
    | CaseObject of UnionMemberName
    with 
        member this.unapply = 
            match this with 
            | CaseClass (name, types) -> UnionMember.apply (name, types) 
            | CaseObject name -> UnionMember.apply (name, [])

    and UnionType = {
        UnionTypeName : UnionTypeName
        TypeArguments : TypeArgument list
        UnionMembers  : UnionMember list
    }
    with 
        static member apply ((unionTypeName, typeArgumentListOption), caseMemberList) = {
            UnionTypeName = unionTypeName
            TypeArguments = typeArgumentListOption |> Option.fold (fun _ s -> s) []
            UnionMembers  = caseMemberList |> List.map (fun (cm : CaseMember) -> cm.unapply)
        }

    and UnionMember = {
        UnionMemberName : UnionMemberName
        UnionMemberParameterTypes : TypeArgument list
    }
    with
        static member apply (name, args) = { UnionMemberName = name; UnionMemberParameterTypes = args }
        member this.unapply = (this.UnionMemberName.unapply, this.UnionMemberParameterTypes |> List.map (fun t -> t.unapply))

    type UsingName = 
    | UsingName of string
    with 
        member this.unapply =  match this with | UsingName x -> x

    type NamespaceMember =
    | Using of UsingName
    | UnionType of UnionType

    type NamespaceName = 
    | NamespaceName of string 
    with 
        member this.unapply =  match this with | NamespaceName x -> x

    type Namespace = | Namespace of (NamespaceName * NamespaceMember list)

[<AutoOpen>]
module Parser = 
    let ws p = spaces >>. p .>> spaces
    let word : Parser<string, unit> = ws (many1Chars asciiLetter)

    let wstr : string -> Parser<string, unit> = pstring >> ws >> attempt
    let braced p = wstr "{" >>. p .>> wstr "}"
    let pointed p = wstr "<" >>. p .>> wstr ">"

    let splitWords sep = sepBy1 word sep
    let comma = wstr ","

    let genericTypeArguments = pointed (splitWords comma) |>> List.map TypeArgument

    let memberName = word |>> UnionMemberName
    let caseClassMember = (wstr "case class" >>. memberName) .>>. genericTypeArguments .>> wstr ";" |>> CaseClass <?> "Case Class"
    let caseObjectMember = wstr "case object" >>. memberName .>> wstr ";" |>> CaseObject <?> "Case Object"

    let bracedMany p = braced (many p)
    let bracedMany1 p = braced (many1 p)
    let caseMember = caseClassMember <|> caseObjectMember
    let caseMembers = (bracedMany1 caseMember) .>> opt (wstr ";")

    let typeName = word |>> UnionTypeName
    let unionType = (wstr "union" >>. typeName) .>>. (opt genericTypeArguments) .>>. caseMembers |>> (UnionType.apply >> UnionType) <?> "Union"

    let namespaceName = word |>> NamespaceName
    let bracedNamedBlock blockTag blockNameParser blockItemParser = 
        (wstr blockTag >>. blockNameParser) .>>. bracedMany blockItemParser

    let usingName = word |>> UsingName
    let using = wstr "using" >>. usingName .>> wstr ";" |>> Using <?> "Using"

    let namespaceMember = using <|> unionType
    let ``namespace`` = bracedNamedBlock "namespace" namespaceName namespaceMember <?> "Namespace"
    
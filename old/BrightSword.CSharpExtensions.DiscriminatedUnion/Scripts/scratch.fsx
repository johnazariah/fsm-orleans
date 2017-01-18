#I __SOURCE_DIRECTORY__
#r "../../packages/FParsec.1.0.2/lib/net40-client/FParsecCS.dll"
#r "../../packages/FParsec.1.0.2/lib/net40-client/FParsec.dll"

open FParsec
open System

type TypeName = | TypeName of string
type TypeArgument = | TypeArgument of string

type MemberName = | MemberName of string
type CaseMember = 
| CaseClass of MemberName * TypeArgument list
| CaseObject of MemberName

type UsingName = | UsingName of string

type NamespaceMember =
| Using of UsingName
| UnionType of (TypeName * TypeArgument list option) * CaseMember list

type NamespaceName = | NamespaceName of string 
type Namespace = | Namespace of (NamespaceName * NamespaceMember list)

let test p str =
    match run p str with
    | Success (result, _, _) -> printfn "Success: %A" result
    | Failure (err, _, _) -> printfn "Failure: %s" err

let expectFailure p str =
    match run p str with
    | Success (result, _, _) -> printfn "Failure: %A" result
    | Failure (err, _, _) -> printfn "As Expected: %A" err

let ws p = spaces >>. p .>> spaces
let word : Parser<string, unit> = ws (many1Chars asciiLetter)
test word "Hello"
test (many word) "Hello World"

let wstr : string -> Parser<string, unit> = pstring >> ws >> attempt
let braced p = wstr "{" >>. p .>> wstr "}"
let pointed p = wstr "<" >>. p .>> wstr ">"
test (pointed word) "<Hello>"
expectFailure (pointed word) "<>" 
expectFailure (pointed word) "<T, R, S>" 

let splitWords sep = sepBy1 word sep
let comma = wstr ","
test (pointed (splitWords comma)) "<T, R, S>"
test (pointed (splitWords comma)) "<T>"
expectFailure (pointed (splitWords comma)) "<T,,R>"
expectFailure (pointed (splitWords comma)) "<T,>"

let genericTypeArguments = pointed (splitWords comma) |>> List.map TypeArgument
test genericTypeArguments "<T,R>"

let memberName = word |>> MemberName
let caseClassMember = (wstr "case class" >>. memberName) .>>. (genericTypeArguments) .>> wstr ";" |>> CaseClass
test caseClassMember "case class Some<T>;"
let caseObjectMember = wstr "case object" >>. memberName .>> wstr ";" |>> CaseObject
test caseObjectMember "case object None;"

let bracedMany p = braced (many p)
let bracedMany1 p = braced (many1 p)
let caseMember = caseClassMember <|> caseObjectMember
let caseMembers = (bracedMany1 caseMember) .>> opt (wstr ";")
test caseMembers "{ case class Some<T>; case object None; }"
test caseMembers "{ case class Some<T>; case object None; };"

let typeName = word |>> TypeName
let unionType = (wstr "union" >>. typeName) .>>. (opt genericTypeArguments) .>>. (caseMembers) |>> UnionType
test unionType "union Option<T> { case class Some<T>; case object None; }"
test unionType "union State { case object Alive; case object Dead; }"

let namespaceName = word |>> NamespaceName
let bracedNamedBlock blockTag blockNameParser blockItemParser = 
    (wstr blockTag >>. blockNameParser) .>>. bracedMany blockItemParser

let usingName = word |>> UsingName
let using = wstr "using" >>. usingName .>> wstr ";" |>> Using
test using "using System;"

let namespaceMember = using <|> unionType

let ``namespace`` = bracedNamedBlock "namespace" namespaceName namespaceMember
test ``namespace`` "namespace Empty {}"
test ``namespace`` "namespace Single { union Option<T> { case class Some<T>; case object None; } }"
test ``namespace`` @"namespace ManyUnions 
{
    using System;

    union Option<T> 
    { 
        case class Some<T>; 
        case object None; 
    };

    union State 
    { 
        case object Alive; 
        case object Dead; 
    }
} "
 
// TODO: Add Constraints

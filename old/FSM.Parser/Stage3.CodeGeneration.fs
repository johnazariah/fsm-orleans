namespace Parser.FSM.Orleans
open System
open System.Reflection
open Microsoft.CodeAnalysis.CSharp.Syntax

[<AutoOpen>]
module CodeGenerator =        
    type DU = {
        DUName : string
        DUEnumerations : DUEnumeration list
        DUConstructors : DUConstructor list
    }
    and DUEnumeration = | EnumerationName of string
    with 
        static member apply(_name) = EnumerationName _name
        member this.unapply = match this with | EnumerationName s -> s

    and DUConstructor = {
        ConstructorName : string
        ConstructorType : string
    }
    with 
        static member apply(_name, _type) = {ConstructorName = _name; ConstructorType = _type}


//        type GenericType = 
//        | Class of GenericClass
//        | Interface of GenericInterface
//        and GenericClass = {
//            ClassName : string
//            TypeArguments : GenericType list
//            Inherits : GenericClass option
//            Implements : GenericInterface list
//            Methods : Method list
//            InnerTypes : GenericType list 
//            IsAbstract : bool
//            IsStatic : bool
//            Visibility : Visibility            
//        }
//        and GenericInterface = {
//            InterfaceName : string
//            TypeArguments : GenericType list
//            Inherits : GenericInterface list
//            Methods : Method list
//            Field : Field list
//        }
//        and Method = {
//            MethodName : string
//            MethodType : GenericType
//            MethodParameters : MethodParameter list
//            MethodBody : MethodBody option
//            IsAbstract : bool
//            IsStatic : bool
//            Visibility : Visibility
//        }
//        and MethodParameter = {
//            MethodParameterName : string
//            MethodParameterType : GenericType
//        }
//        and Field = {
//            FieldName : string
//            FieldType : GenericType
//            FieldInitializer : string option
//            IsAbstract : bool
//            IsStatic : bool
//            IsReadonly : bool
//            Visibility : Visibility
//        }
//        and MethodBody =
//        | MethodBodyExpression of string
//        | MethodBodyBlock of string
//        and Visibility =
//        | Public 
//        | Private
//        | Internal
//        | Protected
//        | ProtectedOrInternal
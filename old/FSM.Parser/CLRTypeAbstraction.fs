namespace Parser.FSM.Orleans

[<AutoOpen>]
module CLRTypeAbstraction =
    open System

    type TypeReference = {
        TypeReferenceName : string
        GenericTypeArguments : TypeReference seq
    }
    with
        static member Zero = {
            TypeReferenceName = String.Empty
            GenericTypeArguments = []
        }         
    and TypeDescription = 
    | Interface of InterfaceDescription
    | Class of ClassDescription
    and InterfaceDescription = {
        InterfaceName : string
        Inherits : TypeReference seq
        GenericTypeArguments : TypeReference seq
        Methods : MethodSignatureDescription seq
        Visibility : Visibility
        InnerInterfaces : InterfaceDescription seq
    }
    with 
        static member Zero = {
            InterfaceName = String.Empty
            Inherits = []
            GenericTypeArguments = []
            Methods = []
            Visibility = Visibility.Public
            InnerInterfaces = []
        }
    and MethodSignatureDescription = {
        MethodSignatureName : string
        MethodSignatureType : TypeReference
        MethodSignatureParameters : ParameterDescription seq
    }
    and ClassDescription = {
        ClassName : string
        Inherits : TypeReference option
        Implements : TypeReference seq
        GenericTypeArguments : TypeReference seq
        ClassMembers : ClassMemberDescription seq
        IsAbstract : bool
        IsStatic : bool
        Visibility : Visibility
    }
    with 
        static member Zero = {
            ClassName = String.Empty
            Inherits = None
            Implements = []
            GenericTypeArguments = []
            ClassMembers = []
            IsAbstract = false
            IsStatic = false
            Visibility = Visibility.Public
        }        
    and ClassMemberDescription = 
    | ClassConstructor of ConstructorDescription
    | ClassProperty of PropertyDescription
    | ClassField of FieldDescription
    | ClassMethod of MethodDescription
    | InnerClass of ClassDescription
    | InnerInterface of InterfaceDescription
    and ConstructorDescription = {
        ConstructorClassName : string
        Visibility : Visibility
        Parameters : ParameterDescription seq
        Body : BodyDescription option
    }
    and FieldDescription = {
        FieldName : string
        IsStatic : bool
        Visibility : Visibility
        IsReadonly : bool
        FieldType : TypeReference
        InitializerExpression : string
    }
    with
        static member Zero = {
            FieldName = String.Empty
            IsStatic = false
            Visibility = Visibility.Public
            IsReadonly = false
            FieldType = TypeReference.Zero
            InitializerExpression = String.Empty
        }
    and PropertyDescription = {
        PropertyName : string
    }
    and MethodDescription = {
        MethodName : string
        MethodType : TypeReference
        GenericTypeArguments : TypeReference seq
        Parameters : ParameterDescription seq
        Body : BodyDescription option
        IsStatic : bool
        IsOverride : bool
        Visibility : Visibility
    }
    with 
        member this.IsAbstract = this.Body.IsNone
        static member Zero = {
            MethodName = String.Empty
            MethodType = TypeReference.Zero
            GenericTypeArguments = []
            Parameters = []
            Body = None
            IsStatic = false
            IsOverride = false
            Visibility = Visibility.Public
        }   
    and BodyDescription =
    | Expression of string
    | Block of string
    and ParameterDescription = {
        ParameterName : string
        ParameterType : TypeReference
        DefaultValue : string option
    }
    with 
        static member Zero = {
            ParameterName = String.Empty
            ParameterType = TypeReference.Zero
            DefaultValue = None
        }
    and Visibility =
    | Public 
    | Private
    | Internal
    | Protected
    | ProtectedOrInternal

    let ``of`` = None
    let ``:`` = None
    let ``,`` = None
    let ``{`` = None
    let ``}`` = None   
    let ``(`` = None
    let ``)`` = None
    let ``[`` = None
    let ``]`` = None
    let ``=>`` = None

    let ``generic type ref`` s ``[`` gats ``]`` = { TypeReferenceName = s; GenericTypeArguments = gats }
    let ``type ref`` s = ``generic type ref`` s ``[`` [] ``]``
    let ``parameter`` n ``of`` t = { ParameterName = n; ParameterType = t; DefaultValue = None }

    let ``method signature`` methodReturnType methodName ``(`` parameters ``)`` =
        {
            MethodSignatureName = methodName
            MethodSignatureType = methodReturnType
            MethodSignatureParameters = parameters
        }

    let ``public generic abstract method`` methodReturnType methodName ``[`` type_args ``]`` ``(`` parameters ``)`` =
        { MethodDescription.Zero with
            MethodName = methodName
            GenericTypeArguments = type_args
            MethodType = methodReturnType
            Parameters = parameters
        }

    let ``public generic override method`` methodReturnType methodName ``[`` type_args ``]`` ``(`` parameters ``)`` ``=>`` expr =
        { MethodDescription.Zero with
            MethodName = methodName
            GenericTypeArguments = type_args
            MethodType = methodReturnType
            Parameters = parameters
            IsOverride = true
            Body = Expression expr |> Some
        }

    let ``public static method`` methodReturnType methodName ``(`` parameters ``)`` ``=>`` expr =
        { MethodDescription.Zero with
            MethodName = methodName
            MethodType = methodReturnType
            Parameters = parameters
            IsStatic = true
            
            Body = Expression expr |> Some
        }
    let ``public generic interface`` interfaceName ``[`` type_args ``]``
        ``:`` inherits 
        ``{`` methods ``}`` = 
        { InterfaceDescription.Zero with
            InterfaceName = interfaceName
            Inherits = inherits
            Methods = methods
            Visibility = Visibility.Public
        }

    let ``public generic class`` className ``[`` type_args ``]``
        ``:`` inherits ``,`` implements 
        ``{`` members ``}`` = 
        { ClassDescription.Zero with
            ClassName = className
            GenericTypeArguments = type_args
            Inherits = inherits
            Implements = implements
            ClassMembers = members
            Visibility = Visibility.Public
        }
        
    let ``public interface`` interfaceName 
        ``:`` inherits 
        ``{`` methods ``}`` = 
        ``public generic interface`` interfaceName ``[`` [] ``]``
            ``:``  inherits
            ``{``
                methods
            ``}``

    let ``public class`` className 
        ``:`` inherits ``,`` implements 
        ``{`` members ``}`` = 
        ``public generic class`` className ``[`` [] ``]``
            ``:``  inherits ``,`` implements
            ``{``
                members
            ``}``

    let ``public abstract class`` className 
        ``:`` inherits ``,`` implements 
        ``{`` members ``}`` = 
        let c = 
            ``public generic class`` className ``[`` [] ``]``
                ``:``  inherits ``,`` implements
                ``{``
                    members
                ``}``
        in { c with IsAbstract = true }

    let ``private static class`` className
        ``{`` members ``}`` = 
        { ClassDescription.Zero with
            ClassName = className
            ClassMembers = members
            Visibility = Visibility.Private
        }

    let ``private empty constructor`` className =
        {  
            ConstructorClassName = className
            Visibility = Visibility.Private
            Parameters = []
            Body = None
        }

    let  ``public static readonly field`` fieldType fieldName ``=>`` initializerExpression =
        { FieldDescription.Zero with
            FieldName = fieldName
            IsStatic = true
            IsReadonly = true
            InitializerExpression = initializerExpression
            FieldType = fieldType
        }

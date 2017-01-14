namespace BrightSword.CSharpExtensions.DiscriminatedUnion

[<AutoOpen>]
module Model =
    open System

    type DiscriminatedUnion = {
        DiscriminatedUnionName : string
        DiscriminatedUnionMembers : DiscriminatedUnionMember seq
        TypeParameters : string seq
        IsSubsetOf : string option
    }
    with 
        static member Zero = {
            DiscriminatedUnionName = String.Empty
            DiscriminatedUnionMembers = []
            TypeParameters = []
            IsSubsetOf = None
        }
        static member apply(_name, _enums) = { 
            DiscriminatedUnion.Zero with
                DiscriminatedUnionName = _name
                DiscriminatedUnionMembers = _enums |> Seq.map (DiscriminatedUnionMember.apply)
        }
        static member apply(_name, _enums, _super) = {
            DiscriminatedUnion.Zero with
                DiscriminatedUnionName = _name
                DiscriminatedUnionMembers = _enums |> Seq.map (DiscriminatedUnionMember.apply)
                IsSubsetOf = Some _super
        }
    and DiscriminatedUnionMember = {
        DiscriminatedUnionMemberName : string
        DiscriminatedUnionMemberType : string list
    }
    with 
        static member apply(_name) = { DiscriminatedUnionMemberName = _name; DiscriminatedUnionMemberType = [] }
        static member apply(_name, _type) = { DiscriminatedUnionMemberName = _name; DiscriminatedUnionMemberType = [_type] }
        static member apply(_name, _types) = { DiscriminatedUnionMemberName = _name; DiscriminatedUnionMemberType = _types }
        member this.unapply = (this.DiscriminatedUnionMemberName, this.DiscriminatedUnionMemberType)

    let fromParsedUnion u = {
        DiscriminatedUnionName = u.UnionTypeName.unapply
        DiscriminatedUnionMembers = u.UnionMembers |> List.map ((fun m -> m.unapply) >> DiscriminatedUnionMember.apply) 
        TypeParameters = u.TypeArguments |> List.map (fun t -> t.unapply)
        IsSubsetOf = None
    }

namespace BrightSword.RoslynWrapper

[<AutoOpen>]
module CompilationUnit =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    
    type SF = SyntaxFactory
    
    let private addMembers members (cu : CompilationUnitSyntax) =
        members
        |> (Seq.toArray >> SF.List)
        |> cu.WithMembers

    let ``compilation unit`` namespaces =
        SF.CompilationUnit()
        |> addMembers namespaces
        |> (fun cu -> cu.NormalizeWhitespace())

    let toCodeString (cu : CompilationUnitSyntax) =
        let fn = Formatting.Formatter.Format (cu, new AdhocWorkspace())
        let sb = new System.Text.StringBuilder()
        use sw = new System.IO.StringWriter (sb)
        fn.WriteTo(sw)
        sb.ToString()
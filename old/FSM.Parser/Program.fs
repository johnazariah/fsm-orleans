namespace Parser.FSM.Orleans

module Program =

    [<EntryPoint>]
    let main argv = 
        BrightSword.CSharpExtensions.DiscriminatedUnion.CodeGeneratorTests.runTests |> ignore
//        ParserTests.runTests |> ignore
//        CodeGeneratorTests.runTests |> ignore

        printfn "%A" argv
        0 // return an integer exit code

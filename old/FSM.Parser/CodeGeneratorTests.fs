namespace Parser.FSM.Orleans

[<AutoOpen>]
module CodeGeneratorTests =
    let runTests = 
        let validMachine = 
            bankAccountStateMachine
            |> parseMachine
            |> Option.map(buildValidMachine)
        
        validMachine |> Option.map(printf "%A") |> ignore

        validMachine 
        |> Option.map (generateGrainInterfaceModule >> BrightSword.RoslynWrapper.CompilationUnit.toCodeString >> printf "%s") 
        |> ignore

        
        validMachine 
        |> Option.map (generateGrainImplementationModule >> BrightSword.RoslynWrapper.CompilationUnit.toCodeString >> printf "%s") 
        |> ignore

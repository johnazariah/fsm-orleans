namespace FSM.Orleans

open BrightSword.RoslynWrapper

[<AutoOpen>]
module CodeGenerator =
    let insert_namespace_into_compilation_unit ns = ``compilation unit`` [ns]

    let generate_code_for_text txt =
        txt
        |> (text_to_valid_machine >> build_namespace >> insert_namespace_into_compilation_unit >> generateCodeToString)


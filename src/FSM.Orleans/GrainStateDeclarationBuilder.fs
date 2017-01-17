namespace FSM.Orleans

[<AutoOpen>]
module GrainStateDeclarationBuilder =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open BrightSword.RoslynWrapper

    let build_grain_state vm =
        let sm = StateMachine vm
        let grainStateClass = sm.grain_state_typename
        let baseClass = sprintf "StateMachineGrainState<%s, %s>" sm.data_typename sm.state_typename |> Some
        let ctor =
            ``constructor`` sm.machine_name ``(`` [("stateMachineData", ``type`` sm.data_typename);("stateMachineState", ``type`` sm.state_typename)] ``)``
                ``:`` ["stateMachineData"; "stateMachineState"]
                [``public``]
                ``{`` [] ``}``
            :> MemberDeclarationSyntax
        in
        [
            ``class`` grainStateClass ``<<`` [] ``>>``
                ``:`` baseClass ``,`` []
                [``public``]
                ``{``
                    [ ctor ]
                ``}``
                :> MemberDeclarationSyntax
        ]
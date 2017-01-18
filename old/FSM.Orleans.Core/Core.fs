namespace FSM.Orleans
      
open System

type InvalidMessage (message) = class inherit Exception (message |> sprintf "Invalid message: %A") end

open Orleans
type StateMachineGrainState<'data, 'stateEnum> = { Data : 'data; CurrentState : 'stateEnum }

type TransitionCondition<'stateEnum, 'message> = { GivenState : 'stateEnum; OnMessage : 'message}
//
//[<AbstractClass>]
//type StateMachine<'data, 'stateEnum, 'message> (data : 'data, initialState : 'stateEnum ) =
//    inherit Orleans.Grain<StateMachineGrainState<'data, 'stateEnum> ()
//
//

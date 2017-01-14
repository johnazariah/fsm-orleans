namespace FSM.BankAccount.Domain

type Amount (value : decimal) =
    do 
        if (value < 0.0M) then failwith "Value cannot be negative"
    member this.Value = value
    override this.Equals that =
        match that with
        | :? Amount as thatAmount -> this.Value = thatAmount.Value
        | _ -> false
    override this.GetHashCode() = this.Value |> hash 
    interface System.IComparable with
        member this.CompareTo that =
            match that with
            | :? Amount as thatAmount-> compare this.Value thatAmount.Value
            | _ -> invalidArg "that" "cannot compare value of different types"
    static member (+) (left : Amount, right : Amount) = Amount (left.Value + right.Value)
    static member (-) (left : Amount, right : Amount) = Amount (left.Value - right.Value)

type Balance =
| ZeroBalance
| ActiveBalance of Amount
| OverdrawnBalance of Amount
with
    member this.Deposit (n : Amount) =
        match this with
        | ZeroBalance -> ActiveBalance n
        | ActiveBalance o -> ActiveBalance (n + o)
        | OverdrawnBalance o -> 
            match (o, n) with
            | (_, _) when (o < n) -> ActiveBalance (n - o) 
            | (_, _) when (o > n) -> OverdrawnBalance (o - n) 
            | _ -> ZeroBalance 
            
    member this.Withdraw (n : Amount) =
        match this with
        | ZeroBalance -> OverdrawnBalance n
        | ActiveBalance o -> 
            match (o, n) with
            | (_, _) when (o < n) -> OverdrawnBalance (n - o) 
            | (_, _) when (o > n) -> ActiveBalance (o - n) 
            | _ -> ZeroBalance 
        | OverdrawnBalance o ->  OverdrawnBalance (n + o) 


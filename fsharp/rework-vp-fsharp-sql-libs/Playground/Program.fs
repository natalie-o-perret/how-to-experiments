open System

open Vp.FSharp.Sql.Sqlite


sqliteCommand {
    text ""
    noLogger
} |> printfn "%A"

SqliteCommand.initWithText ""
|> SqliteCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"

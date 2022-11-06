open System

open Vp.FSharp.Sql
open Vp.FSharp.Sql.Postgres
open Vp.FSharp.Sql.Sqlite


sqliteCommand {
    text ""
    noLogger
} |> printfn "%A"

SqliteCommand.text ""
|> SqliteCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"

postgresCommand {
    text ""
    noLogger
} |> printfn "%A"

PostgresCommand.text ""
|> PostgresCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"

open System

open Vp.FSharp.Sql.Sqlite
open Vp.FSharp.Sql.Postgres
open Vp.FSharp.Sql.SqlServer


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

sqlServerCommand {
    text ""
    noLogger
} |> printfn "%A"

SqlServerCommand.text ""
|> SqlServerCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"

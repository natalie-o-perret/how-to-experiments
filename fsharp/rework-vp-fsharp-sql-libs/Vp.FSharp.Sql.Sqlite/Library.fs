module Vp.FSharp.Sql.Sqlite

open System.Data
open System.Data.SQLite
open System.Threading.Tasks

open Vp.FSharp.Sql


type SqliteDbValue =
    | Null
    | Integer of int64
    | Real of double
    | Text of string
    | Blob of byte array
    | Custom of DbType * obj
    interface IDbValue<SqliteDbValue, SQLiteParameter> with
        member this.ToParameter name =
            let parameter = SQLiteParameter()
            parameter.ParameterName <- name
            match this with
            | Null ->
                parameter.TypeName <- "NULL"
            | Integer value ->
                parameter.TypeName <- "INTEGER"
                parameter.Value    <- value
            | Real value ->
                parameter.TypeName <- "REAL"
                parameter.Value    <- value
            | Text value ->
                parameter.TypeName <- "TEXT"
                parameter.Value    <- value
            | Blob value ->
                parameter.TypeName <- "BLOB"
                parameter.Value    <- value

            | Custom (dbType, value) ->
                parameter.DbType <- dbType
                parameter.Value  <- value

            parameter


type SqliteCommandDefinition =
    CommandDefinition<SQLiteConnection, SQLiteCommand, SQLiteParameter, SQLiteDataReader, SQLiteTransaction, SqliteDbValue, SqliteIODependencies, SqliteCommandDefinition>

and SqliteIODependencies () =
    interface IDependencies<SQLiteConnection, SQLiteCommand, SQLiteParameter, SQLiteDataReader, SQLiteTransaction, SqliteDbValue, SqliteIODependencies, SqliteCommandDefinition> with
        member this.CreateCommand(connection: SQLiteConnection) = connection.CreateCommand()
        member this.SetCommandTransaction (command: SQLiteCommand) transaction = command.Transaction <- transaction

        member this.BeginTransaction (connection: SQLiteConnection) isolationLevel =
            connection.BeginTransaction(isolationLevel = isolationLevel)

        member this.BeginTransactionTask (connection: SQLiteConnection) isolationLevel _ =
            ValueTask.FromResult(connection.BeginTransaction(isolationLevel = isolationLevel))

        member this.ExecuteReader(command: SQLiteCommand) = command.ExecuteReader()

        member this.ExecuteReaderTask (command: SQLiteCommand) _ =
            Task.FromResult(command.ExecuteReader())

        member this.CastCommandDefinition (input: SqliteCommandDefinition) =
            input

[<Sealed>]
type SqliteCommand private () =
    inherit SqlCommand<SQLiteConnection, SQLiteCommand, SQLiteParameter, SQLiteDataReader, SQLiteTransaction, SqliteDbValue, SqliteIODependencies, SqliteCommandDefinition>()

type SqliteCommandBuilder() =

    inherit SqlCommandBuilder<SQLiteConnection, SQLiteCommand, SQLiteParameter, SQLiteDataReader, SQLiteTransaction, SqliteDbValue, SqliteIODependencies, SqliteCommandDefinition>()


let sqliteCommand = SqliteCommandBuilder()

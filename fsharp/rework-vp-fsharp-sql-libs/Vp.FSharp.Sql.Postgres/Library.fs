module Vp.FSharp.Sql.Postgres

open System
open System.Data
open System.Net
open System.Collections.Generic
open System.Net.NetworkInformation

open Npgsql
open NpgsqlTypes

open Vp.FSharp.Sql
open Vp.FSharp.Sql.Postgres


type PostgresCommandDefinition = CommandDefinition<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlDataReader, NpgsqlTransaction, PostgresDbValue>


type PostgresIODependencies private () =
    interface IDependencies<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlDataReader, NpgsqlTransaction, PostgresDbValue, PostgresCommandDefinition> with
        member this.CreateCommand (connection: NpgsqlConnection) =
            connection.CreateCommand()
        member this.SetCommandTransaction (command: NpgsqlCommand) transaction =
            command.Transaction <- transaction
        member this.BeginTransaction (connection: NpgsqlConnection) isolationLevel =
            connection.BeginTransaction(isolationLevel)
        member this.BeginTransactionTask (connection: NpgsqlConnection) isolationLevel cancellationToken =
            connection.BeginTransactionAsync(isolationLevel, cancellationToken)
        member this.ExecuteReader (command: NpgsqlCommand) =
            command.ExecuteReader(CommandBehavior.Default)
        member this.ExecuteReaderTask (command: NpgsqlCommand) cancellationToken =
            command.ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)



[<Sealed>]
type PostgresCommand private () =
    inherit SqlCommand<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlDataReader, NpgsqlTransaction, PostgresDbValue, PostgresIODependencies, PostgresCommandDefinition>()


type PostgresCommandBuilder() =

    inherit SqlCommandBuilder<TConnection, TCommand, TParameter, TDataReader, TTransaction, PostgresDbValue, PostgresIODependencies>()
    do ()


let postgresCommand = PostgresCommandBuilder()

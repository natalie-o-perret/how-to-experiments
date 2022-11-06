module Vp.FSharp.Sql.Sqlite

open System.Data
open System.Data.SQLite

open Vp.FSharp.Sql
open Vp.FSharp.Sql.Sqlite.StaticAbstracts


type SqliteDbValue =
    | Null
    | Integer of int64
    | Real of double
    | Text of string
    | Blob of byte array
    | Custom of DbType * obj
    interface IDbValue<SqliteDbValue, SQLiteParameter> with
        member this.ToParameter name value =

            let parameter = SQLiteParameter()
            parameter.ParameterName <- name
            match value with
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


type TConnection = SQLiteConnection
type TCommand = SQLiteCommand
type TParameter = SQLiteParameter
type TDataReader = SQLiteDataReader
type TTransaction = SQLiteTransaction


[<Sealed>]
type SqliteCommand private () =
    inherit SqlCommand<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqliteDbValue, SqliteIODependencies>()

type SqliteCommandBuilder() =

    inherit SqlCommandBuilder<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqliteDbValue, SqliteIODependencies>()
    do ()


let sqliteCommand = SqliteCommandBuilder()

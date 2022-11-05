module Vp.FSharp.Sql.Sqlite

open System.Data
open System.Data.SQLite

open Vp.FSharp.Sql


type SqliteDbValue =
    | Null
    | Integer of int64
    | Real of double
    | Text of string
    | Blob of byte array
    | Custom of DbType * obj


type TConnection = SQLiteConnection
type TCommand = SQLiteCommand
type TParameter = SQLiteParameter
type TDataReader = SQLiteDataReader
type TTransaction = SQLiteTransaction

[<Sealed>]
type SqliteCommand private () =
    inherit SqlCommand<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqliteDbValue>()

type SqliteCommandBuilder() =

    inherit SqlCommandBuilder<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqliteDbValue>()
    do ()


let sqliteCommand = SqliteCommandBuilder()

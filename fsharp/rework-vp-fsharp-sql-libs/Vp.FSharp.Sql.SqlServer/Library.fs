module Vp.FSharp.Sql.SqlServer

open System
open System.Data
open System.Data.SqlTypes

open Vp.FSharp.Sql



/// Native SQL Server DB types.
/// See https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
/// and https://stackoverflow.com/a/968734/4636721
type SqlServerDbValue =
    | Null

    | Bit of bool

    | TinyInt of uint8
    | SmallInt of int16
    | Int of int32
    | BigInt of int64

    | Real of single
    | Float of double

    | SmallMoney of decimal
    | Money of decimal
    | Decimal of decimal
    | Numeric of decimal

    | Binary of uint8 array
    | VarBinary of uint8 array
    | Image of uint8 array
    | RowVersion of uint8 array
    | FileStream of uint8 array
    | Timestamp of uint8 array

    | UniqueIdentifier of Guid

    | Time of TimeSpan
    | Date of DateTime
    | SmallDateTime of DateTime
    | DateTime of DateTime
    | DateTime2 of DateTime
    | DateTimeOffset of DateTimeOffset

    | Char of string
    | NChar of string
    | VarChar of string
    | NVarChar of string
    | Text of string
    | NText of string

    | Xml of SqlXml

    | SqlVariant of obj

    | Custom of (SqlDbType * obj)


type TConnection = Microsoft.Data.SqlClient.SqlConnection
type TCommand = Microsoft.Data.SqlClient.SqlCommand
type TParameter = Microsoft.Data.SqlClient.SqlParameter
type TDataReader = Microsoft.Data.SqlClient.SqlDataReader
type TTransaction = Microsoft.Data.SqlClient.SqlTransaction

[<Sealed>]
type SqlServerCommand private () =
    inherit SqlCommand<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqlServerDbValue>()

type SqlServerCommandBuilder() =

    inherit SqlCommandBuilder<TConnection, TCommand, TParameter, TDataReader, TTransaction, SqlServerDbValue>()
    do ()


let sqlServerCommand = SqlServerCommandBuilder()

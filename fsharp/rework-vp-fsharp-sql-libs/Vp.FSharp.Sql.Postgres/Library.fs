module Vp.FSharp.Sql.Postgres

open System
open System.Net
open System.Collections.Generic
open System.Net.NetworkInformation

open Npgsql
open NpgsqlTypes

open Vp.FSharp.Sql


/// Native PostgreSQL DB types.
/// See https://www.npgsql.org/doc/types/basic.html
/// and https://stackoverflow.com/a/845472/4636721
type PostgresValue =
    | Null

    | Bit of bool
    | Boolean of bool

    | SmallInt of int16
    | Integer of int32
    | Oid of uint32
    | Xid of uint32
    | Cid of uint32
    | BigInt of int64

    | Real of single
    | Double of double

    | Money of decimal
    | Numeric of decimal

    | ByteA of uint8 array
    | OidVector of uint32 array

    | Uuid of Guid

    | INet of IPAddress
    | MacAddr of PhysicalAddress

    | TsQuery of NpgsqlTsQuery
    | TsVector of NpgsqlTsVector

    | Point of NpgsqlPoint
    | LSeg of NpgsqlLSeg
    | Path of NpgsqlPath
    | Polygon of NpgsqlPolygon
    | Line of NpgsqlLine
    | Circle of NpgsqlCircle
    | Box of NpgsqlBox

    | HStore of Dictionary<string, string>

    | Date of DateTime
    | Interval of TimeSpan
    | Time of DateTime
    | TimeTz of DateTime
    | Timestamp of DateTime
    | TimestampTz of DateTimeOffset

    | InternalChar of uint8

    | Char of string
    | VarChar of string

    | Name of string
    | CiText of string
    | Text of string
    | Xml of string
    | Json of string
    | Jsonb of string

    /// Only if the relevant Npgsql mapping for the Enum has been set up beforehand.
    /// See: https://www.npgsql.org/doc/types/enums_and_composites.html
    | Enum of Enum

    | Custom of (NpgsqlDbType * obj)


type TConnection = NpgsqlConnection
type TCommand = NpgsqlCommand
type TParameter = NpgsqlParameter
type TDataReader = NpgsqlDataReader
type TTransaction = NpgsqlTransaction

[<Sealed>]
type PostgresCommand private () =
    inherit SqlCommand<TConnection, TCommand, TParameter, TDataReader, TTransaction, PostgresValue>()

type PostgresCommandBuilder() =

    inherit SqlCommandBuilder<TConnection, TCommand, TParameter, TDataReader, TTransaction, PostgresValue>()
    do ()


let postgresCommand = PostgresCommandBuilder()

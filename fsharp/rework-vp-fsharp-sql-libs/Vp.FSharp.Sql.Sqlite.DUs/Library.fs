namespace Vp.FSharp.Sql.Sqlite.DUs

open System.Data


type SqliteDbValue =
    | Null
    | Integer of int64
    | Real of double
    | Text of string
    | Blob of byte array
    | Custom of DbType * obj
    interface IDbValue<'T, 'Parameter>

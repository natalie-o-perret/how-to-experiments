module internal Vp.FSharp.Sql.Helpers

open System


module DbNull =
    let is<'T> () = typedefof<'T> = typedefof<DBNull>

    let retypedAs<'T> () = DBNull.Value :> obj :?> 'T

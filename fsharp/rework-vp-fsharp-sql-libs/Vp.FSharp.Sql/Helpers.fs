module Vp.FSharp.Sql.Helpers

open System


module DbNull =
    let is<'T> () = typedefof<'T> = typedefof<DBNull>

    let retypedAs<'T> () = DBNull.Value :> obj :?> 'T

type Singleton<'T when 'T : (new : unit -> 'T)> private () =
    static let instance = lazy new 'T()
    static member Instance with get() = instance.Value

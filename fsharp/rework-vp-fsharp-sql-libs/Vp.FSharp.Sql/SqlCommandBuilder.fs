namespace Vp.FSharp.Sql

open System.Data.Common

open Vp.FSharp.Sql
open Vp.FSharp.Sql.Helpers


[<AbstractClass>]
type SqlCommandBuilder<'Connection, 'Command, 'Parameter, 'DataReader, 'Transaction, 'DbValue, 'Dependencies, 'CommandDefinition
    when 'Connection :> DbConnection
    and 'Command :> DbCommand
    and 'Parameter :> DbParameter
    and 'DataReader :> DbDataReader
    and 'Transaction :> DbTransaction
    and 'DbValue :> IDbValue<'DbValue, 'Parameter>
    and 'Dependencies :> IDependencies<'Connection, 'Command, 'Parameter, 'DataReader, 'Transaction, 'DbValue, 'Dependencies, 'CommandDefinition>
    and 'Dependencies: (new: unit -> 'Dependencies)
    > () =

    member _.Yield _ =
        SqlCommand.init ()
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Initialize a new command definition with the given text contained in the given string.
    [<CustomOperation("text")>]
    member _.SetText(definition, value) =
        SqlCommand.text value definition

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    [<CustomOperation("textFromList")>]
    member _.SetTextFromList(definition, value) =
        SqlCommand.textFromList value definition

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("noLogger")>]
    member _.SetNoLogger(definition) =
        SqlCommand.noLogger definition

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("overrideLogger")>]
    member _.SetOverrideLogger(definition, value) =
        SqlCommand.overrideLogger value definition

    /// Update the command definition with the given parameters.
    [<CustomOperation("parameters")>]
    member _.SetParameters(definition, value) =
        SqlCommand.parameters value definition

    /// Update the command definition with the cancellation token.
    [<CustomOperation("cancellationToken")>]
    member _.SetCancellationToken(definition, value) =
        SqlCommand.cancellationToken value definition

    /// Update the command definition with the given timeout.
    [<CustomOperation("timeout")>]
    member _.SetTimeout(definition, value) =
        SqlCommand.timeout value definition

    /// Update the command definition with the given command type (ie. how it should be interpreted).
    [<CustomOperation("commandType")>]
    member _.SetCommandType(definition, value) =
        SqlCommand.commandType value definition

    /// Update the command definition and sets whether the command should be prepared or not.
    [<CustomOperation("prepare")>]
    member _.SetPrepare(definition, value) =
        SqlCommand.prepare value definition

    /// Update the command definition and sets whether the command should be wrapped in the given transaction.
    [<CustomOperation("transaction")>]
    member _.SetTransaction(definition, value) =
        SqlCommand.transaction value definition

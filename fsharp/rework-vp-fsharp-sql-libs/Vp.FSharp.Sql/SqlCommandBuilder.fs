namespace Vp.FSharp.Sql

open System.Data.Common

open Vp.FSharp.Sql
open Vp.FSharp.Sql.StaticAbstracts


[<AbstractClass>]
type SqlCommandBuilder<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies
    when 'TConnection :> DbConnection
    and 'TCommand :> DbCommand
    and 'TParameter :> DbParameter
    and 'TDataReader :> DbDataReader
    and 'TTransaction :> DbTransaction
    and 'TDbValue :> IDbValue<'TDbValue, 'TParameter>
    and 'TIODependencies :> IIODependencies<'TConnection, 'TCommand, 'TDataReader, 'TTransaction>
    >() =

    member _.Yield _ =
        SqlCommand.init ()
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Initialize a new command definition with the given text contained in the given string.
    [<CustomOperation("text")>]
    member _.SetText(definition, value) =
        SqlCommand.setText value definition
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    [<CustomOperation("textFromList")>]
    member _.SetTextFromList(definition, value) =
        SqlCommand.setTextFromList value definition
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("noLogger")>]
    member _.SetNoLogger(definition) =
        SqlCommand.noLogger definition
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("overrideLogger")>]
    member _.SetOverrideLogger(definition, value) =
        SqlCommand.overrideLogger value definition
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("overrideLogger")>]
    member _.parameters(definition, value) =
        SqlCommand.overrideLogger value definition
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

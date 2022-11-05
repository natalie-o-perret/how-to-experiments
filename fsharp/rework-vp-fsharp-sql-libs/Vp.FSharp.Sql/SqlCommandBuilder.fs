namespace Vp.FSharp.Sql

open System.Data.Common

open Vp.FSharp.Sql


[<AbstractClass>]
type SqlCommandBuilder<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType
    when 'DbConnection :> DbConnection
    and 'DbCommand :> DbCommand
    and 'DbParameter :> DbParameter
    and 'DbDataReader :> DbDataReader
    and 'DbTransaction :> DbTransaction>() =

    member _.Yield _ =
        SqlCommand.init ()
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

    /// Initialize a new command definition with the given text contained in the given string.
    [<CustomOperation("text")>]
    member _.SetText(definition, value) =
        SqlCommand.setText value definition
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    [<CustomOperation("textFromList")>]
    member _.SetTextFromList(definition, value) =
        SqlCommand.setTextFromList value definition
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("noLogger")>]
    member _.SetNoLogger(definition) =
        SqlCommand.noLogger definition
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("overrideLogger")>]
    member _.SetOverrideLogger(definition, value) =
        SqlCommand.overrideLogger value definition
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    [<CustomOperation("overrideLogger")>]
    member _.parameters(definition, value) =
        SqlCommand.overrideLogger value definition
        : CommandDefinition<'DbConnection, 'DbCommand, 'DbParameter, 'DbDataReader, 'DbTransaction, 'DbType>

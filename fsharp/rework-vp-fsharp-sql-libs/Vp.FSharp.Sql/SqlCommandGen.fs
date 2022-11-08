namespace Vp.FSharp.Sql

open System.Data.Common

open Vp.FSharp.Sql.Helpers


type SqlCommand<'Connection, 'Command, 'Parameter, 'DataReader, 'Transaction, 'DbValue, 'Dependencies, 'CommandDefinition
    when 'Connection :> DbConnection
    and 'Command :> DbCommand
    and 'Parameter :> DbParameter
    and 'DataReader :> DbDataReader
    and 'Transaction :> DbTransaction
    and 'DbValue :> IDbValue<'DbValue, 'Parameter>
    and 'Dependencies :> IDependencies<'Connection, 'Command, 'Parameter, 'DataReader, 'Transaction, 'DbValue, 'Dependencies,'CommandDefinition>
    and 'Dependencies: (new: unit -> 'Dependencies)
    > () =


    /// Update the command definition with the given text contained in the given string.
    static member inline text value definition =
        SqlCommand.text value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition with the given text spanning over several strings (ie. list).
    static member inline textFromList value definition =
        SqlCommand.textFromList value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Initialize a new command definition with the given text contained in the given string.
    static member inline initWithText value =
        SqlCommand.initWithText value
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    static member inline initWithTextFromList value =
        SqlCommand.initWithTextFromList value
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    static member inline noLogger definition =
        SqlCommand.noLogger definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition so that when executing the command, it use the given overriding logger.
    /// instead of the default one, aka the Global logger, if any.
    static member inline overrideLogger value definition =
        SqlCommand.overrideLogger value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition with the given parameters.
    static member inline parameters value definition =
        SqlCommand.parameters value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition with the given cancellation token.
    static member inline cancellationToken value definition =
        SqlCommand.cancellationToken value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition with the given timeout.
    static member inline timeout value definition =
        SqlCommand.timeout value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition and sets the command type (ie. how it should be interpreted).
    static member inline commandType value definition =
        SqlCommand.commandType value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition and sets whether the command should be prepared or not.
    static member inline prepare value definition =
        SqlCommand.prepare value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    /// Update the command definition and sets whether the command should be wrapped in the given transaction.
    static member inline transaction value definition =
        SqlCommand.transaction value definition
        |> Singleton<'Dependencies>.Instance.CastCommandDefinition

    static member executeWhatever connection =
        use command = Singleton<'Dependencies>.Instance.CreateCommand connection
        42

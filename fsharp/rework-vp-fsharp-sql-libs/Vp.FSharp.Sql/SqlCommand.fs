namespace Vp.FSharp.Sql

open System
open System.Data
open System.Threading
open System.Data.Common

open Vp.FSharp.Sql.StaticAbstracts


[<RequireQualifiedAccess>]
module SqlCommand =

    let [<Literal>] DefaultTimeoutInSeconds = 30.
    let [<Literal>] DefaultPrepare = false
    let [<Literal>]  DefaultCommandType = CommandType.Text


[<AbstractClass>]
type SqlCommand<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies
    when 'TConnection :> DbConnection
    and 'TCommand :> DbCommand
    and 'TParameter :> DbParameter
    and 'TDataReader :> DbDataReader
    and 'TTransaction :> DbTransaction
    and 'TDbValue :> IDbValue<'TDbValue, 'TParameter>
    and 'TIODependencies :> IIODependencies<'TConnection, 'TCommand, 'TDataReader, 'TTransaction>> () =

    static member val DefaultTimeoutInSeconds = 30.
    static member val DefaultPrepare = false
    static member val DefaultCommandType = CommandType.Text


    static member internal init() =
        { Text = Text.Single String.Empty
          Parameters = []
          CancellationToken = CancellationToken.None
          Timeout = TimeSpan.FromSeconds(SqlCommand.DefaultTimeoutInSeconds)
          CommandType = SqlCommand.DefaultCommandType
          Prepare = SqlCommand.DefaultPrepare
          Transaction = None
          Logger = LoggerKind.Configuration }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition with the given text contained in the given string.
    static member internal setText value definition =
        { definition with Text = Text.Single value }
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition with the given text spanning over several strings (ie. list).
    static member internal setTextFromList value definition =
        { definition with Text = Text.Multiple value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Initialize a new command definition with the given text contained in the given string.
    static member text value =
        SqlCommand<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>.init ()
        |> SqlCommand<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>.setText value

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    static member textFromList value =
        SqlCommand<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>.init ()
        |> SqlCommand<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>.setTextFromList
            value

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    static member noLogger commandDefinition =
        { commandDefinition with Logger = Nothing }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition so that when executing the command, it use the given overriding logger.
    /// instead of the default one, aka the Global logger, if any.
    static member overrideLogger value commandDefinition =
        { commandDefinition with Logger = LoggerKind.Override value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition with the given parameters.
    static member parameters value commandDefinition =
        { commandDefinition with Parameters = value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition with the given cancellation token.
    static member cancellationToken value commandDefinition =
        { commandDefinition with CancellationToken = value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition with the given timeout.
    static member timeout value commandDefinition =
        { commandDefinition with Timeout = value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition and sets the command type (ie. how it should be interpreted).
    static member commandType value commandDefinition =
        { commandDefinition with CommandType = value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition and sets whether the command should be prepared or not.
    static member prepare value commandDefinition =
        { commandDefinition with Prepare = value }: CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    /// Update the command definition and sets whether the command should be wrapped in the given transaction.
    static member transaction value commandDefinition =
        { commandDefinition with Transaction = Some value }
        : CommandDefinition<'TConnection, 'TCommand, 'TParameter, 'TDataReader, 'TTransaction, 'TDbValue, 'TIODependencies>

    static member executeWhatever connection =
        use command = SqlDependencyFSharpBridge<'TConnection, 'TCommand, 'TDataReader, 'TTransaction, 'TIODependencies>
                          .CreateCommand(connection)
        42

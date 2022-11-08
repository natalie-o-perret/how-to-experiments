namespace Vp.FSharp.Sql

open System
open System.Data
open System.Threading


[<RequireQualifiedAccess>]
module SqlCommand =

    [<Literal>]
    let DefaultTimeoutInSeconds = 30.

    [<Literal>]
    let DefaultCommandType = CommandType.Text

    [<Literal>]
    let DefaultPrepare = false

    // Init the default definition
    let inline init () =
        { Text = Text.Single String.Empty
          Parameters = []
          CancellationToken = CancellationToken.None
          Timeout = TimeSpan.FromSeconds(DefaultTimeoutInSeconds)
          CommandType = DefaultCommandType
          Prepare = DefaultPrepare
          Transaction = None
          Logger = LoggerKind.Configuration }

    /// Update the command definition with the given text contained in the given string.
    let inline text value definition =
        { definition with Text = Text.Single value }

    /// Update the command definition with the given text spanning over several strings (ie. list).
    let inline textFromList value definition =
        { definition with Text = Text.Multiple value }

    /// Initialize a new command definition with the given text contained in the given string.
    let inline initWithText value = init () |> text value

    /// Initialize a new command definition with the given text spanning over several strings (ie. list).
    let inline initWithTextFromList value = init () |> textFromList value

    /// Update the command definition so that when executing the command, it doesn't use any logger.
    /// Be it the default one (Global, if any.) or a previously overriden one.
    let inline noLogger definition = { definition with Logger = Nothing }

    /// Update the command definition so that when executing the command, it use the given overriding logger.
    /// instead of the default one, aka the Global logger, if any.
    let inline overrideLogger value definition =
        { definition with Logger = LoggerKind.Override value }

    /// Update the command definition with the given parameters.
    let inline parameters value definition = { definition with Parameters = value }

    /// Update the command definition with the given cancellation token.
    let inline cancellationToken value commandDefinition =
        { commandDefinition with CancellationToken = value }

    /// Update the command definition with the given timeout.
    let inline timeout value commandDefinition =
        { commandDefinition with Timeout = value }

    /// Update the command definition and sets the command type (ie. how it should be interpreted).
    let inline commandType value commandDefinition =
        { commandDefinition with CommandType = value }

    /// Update the command definition and sets whether the command should be prepared or not.
    let inline prepare value definition =
        { definition with Prepare = value }

    /// Update the command definition and sets whether the command should be wrapped in the given transaction.
    let inline transaction value definition =
        { definition with Transaction = Some value }

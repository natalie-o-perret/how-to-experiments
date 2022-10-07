namespace HowToAddHangfireConsoleLogs.Helpers.Log

open Nito.Disposables

open Serilog
open Serilog.Context

open Serilog.Events
open Serilog.Filters
open Serilog.Formatting.Compact
open Serilog.Sinks.SystemConsole.Themes
open HowToAddHangfireConsoleLogs.Helpers.Logging


type LogPropertyName  = string
type LogPropertyValue = string
type LogProperty      = LogPropertyName * LogPropertyValue

type LogSettings =
    { AppName:     string
      MinLogLevel: LogEventLevel
      Properties:  LogProperty list }

[<RequireQualifiedAccess>]
module Log = 
    let private addSinkConsole useJsonCompactFormat (loggerConfiguration: LoggerConfiguration) =
        if useJsonCompactFormat then loggerConfiguration.WriteTo.Console(RenderedCompactJsonFormatter())
        else loggerConfiguration.WriteTo.Console(theme=AnsiConsoleTheme.Code, applyThemeToRedirectedOutput=true)
    
    let private enrichWithProperties (properties: LogProperty list) source =
        properties
        |> List.fold(fun (state: LoggerConfiguration) -> state.Enrich.WithProperty) source
    
    let private exclusionPredicate =
        let excludeRequestStartingWith (requestPath: string) = 
            requestPath.StartsWith("/health") || 
            requestPath.StartsWith("/hangfire")
        Matching.WithProperty("RequestPath", excludeRequestStartingWith)
    
    let applyDefault settings (config: LoggerConfiguration) =
        config.Destructure.FSharpTypes()
              .MinimumLevel.Is(settings.MinLogLevel)
              .Filter.ByExcluding(exclusionPredicate)
              .Enrich.WithAssemblyName()
              .Enrich.WithEnvironmentName()
              .Enrich.WithMachineName()
              .Enrich.FromLogContext()
              |> HangfireConsoleLoggerConfiguration.enrichWithHangfireContext
              |> enrichWithProperties settings.Properties
              |> addSinkConsole true


    let enrichWith name value = LogContext.PushProperty(name, value)

    let write level message (args: obj list) = Log.Write(level, message, Array.ofList args)
    let writeWith level message (args: obj list) (properties: (string * obj) list) =
        let properties = List.map (fun (name, value) -> enrichWith name value) properties
        use disposeProperties = new CollectionDisposable(properties)
        write level message args

    let writeExn level e message (args: obj list) = Log.Write(level, e, message, Array.ofList args)
    let writeExnWith level e message (args: obj list) (properties: (string * obj) list)  =
        let properties = List.map (fun (name, value) -> enrichWith name value) properties
        use disposeProperties = new CollectionDisposable(properties)
        writeExn level e message args

    let verbose = write LogEventLevel.Verbose
    let verboseWith = writeWith LogEventLevel.Verbose

    let debug = write LogEventLevel.Debug
    let debugWith = writeWith LogEventLevel.Debug

    let information = write LogEventLevel.Information
    let informationWith = writeWith LogEventLevel.Information

    let warning = write LogEventLevel.Warning
    let warningWith = writeWith LogEventLevel.Warning
    
    let error = write LogEventLevel.Error
    let errorWith = write LogEventLevel.Error
    
    let errorExn e = writeExn LogEventLevel.Error e
    let errorExnWith e = writeExnWith LogEventLevel.Error e

    let fatal = write LogEventLevel.Fatal
    let fatalWith = writeWith LogEventLevel.Fatal

    let fatalExn e = writeExn LogEventLevel.Fatal e
    let fatalExnWith e = writeExnWith LogEventLevel.Fatal e


namespace HowToAddHangfireConsoleLogs.Helpers.Logging


open Serilog
open Serilog.Events

open HowToAddHangfireConsoleLogs.Helpers.Logging.Hangfire.Implementation


[<RequireQualifiedAccess>]
module HangfireConsoleLoggerConfiguration = 

    let [<Literal>] DefaultMinimumLevel = LogEventLevel.Verbose
    
    let enrichWithHangfireContext (config: LoggerConfiguration) =
        config.Enrich.With<HangfireConsoleSerilogEnricher>()

    let addSinkHangfireConsole (config: LoggerConfiguration) =
        config.WriteTo.Sink(HangfireConsoleSink(null))
        
    let apply minimumLogEventLevel (config: LoggerConfiguration) =
        config.Destructure.FSharpTypes()
              .MinimumLevel.Is(minimumLogEventLevel)
              |> enrichWithHangfireContext
    
    let createDefault() =
        LoggerConfiguration()
        |> apply DefaultMinimumLevel 
    
[<AbstractClass; Sealed>]
type HangfireConsoleLog private () = 
    
    static let createDefaultLogger() =
        let loggerConfiguration = HangfireConsoleLoggerConfiguration.createDefault()
        loggerConfiguration.CreateLogger()
    
    static member val internal Logger = createDefaultLogger()
        
[<RequireQualifiedAccess>]
module HangfireConsoleLog = 
    
    let write level message (args: obj list) =
        HangfireConsoleLog.Logger.Write(level, message, Array.ofList args)
    let writeExn level e message (args: obj list) =
        HangfireConsoleLog.Logger.Write(level, e, message, Array.ofList args)

    let verbose     = write LogEventLevel.Verbose
    let debug       = write LogEventLevel.Debug
    let information = write LogEventLevel.Information
    let warning     = write LogEventLevel.Warning
    let error       = write LogEventLevel.Error
    let errorWith   = write LogEventLevel.Error
    let errorExn e  = writeExn LogEventLevel.Error e
    let fatal       = write LogEventLevel.Fatal
    let fatalExn e  = writeExn LogEventLevel.Fatal e

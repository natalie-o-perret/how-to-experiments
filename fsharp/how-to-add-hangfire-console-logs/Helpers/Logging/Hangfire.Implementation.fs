namespace HowToAddHangfireConsoleLogs.Helpers.Logging.Hangfire.Implementation

open System
open System.Threading
open System.Threading.Tasks
open System.Linq.Expressions

open Microsoft.Extensions.Logging

open Serilog.Core
open Serilog.Events

open Hangfire
open Hangfire.Common
open Hangfire.Server
open Hangfire.Console
open Hangfire.Console.Progress


type IPerformingContextAccessor =
    abstract member TryGet : unit -> PerformingContext option
    abstract member Iter   : (PerformingContext -> unit)-> unit
    abstract member Map    : (PerformingContext -> 'T) -> 'T option

type PerformingContextAccessor() =
    interface IPerformingContextAccessor with
        member this.TryGet() = HangfireSubscriberServerFilter.Value
        member this.Iter(f)  = Option.iter f HangfireSubscriberServerFilter.Value 
        member this.Map(f)   = Option.map f HangfireSubscriberServerFilter.Value 

and HangfireSubscriberServerFilter() =
    
    static let localStorage = AsyncLocal<PerformingContext>()
    
    static member Value with get() = Option.ofObj localStorage.Value
    
    interface IServerFilter with
    
        member this.OnPerforming(performingContext) = localStorage.Value <- performingContext
        member this.OnPerformed _                   = localStorage.Value <- null

[<RequireQualifiedAccess>]
module PerformingContextStructureValue =
    
    let private getBasicProperties (backgroundJob: BackgroundJob) = List.map LogEventProperty <| [
        "id",        ScalarValue backgroundJob.Id
        "createdAt", ScalarValue backgroundJob.CreatedAt
    ]
    let private getScalarValue (value: obj) = 
        if (not (isNull value) && not (value.GetType().IsPrimitive)) then ScalarValue(value.ToString())
        else ScalarValue(value)
        :> LogEventPropertyValue
    let private getExtendedProperties (job: Job) = List.map LogEventProperty <| [
        "type",      job.Method.DeclaringType.Name      |> ScalarValue   :> LogEventPropertyValue
        "method",    job.Method.Name                    |> ScalarValue   :> LogEventPropertyValue
        "arguments", job.Args |> Seq.map getScalarValue |> SequenceValue :> LogEventPropertyValue
    ] 
    let createProperties (performingContext: PerformingContext) = [
        if isNull performingContext.BackgroundJob.Job then
            yield! getBasicProperties    performingContext.BackgroundJob
        else
            yield! getBasicProperties    performingContext.BackgroundJob
            yield! getExtendedProperties performingContext.BackgroundJob.Job
    ]

type PerformingContextStructureValue (performingContext) =
    inherit StructureValue(PerformingContextStructureValue.createProperties performingContext)
    member this.PerformingContext with get() = performingContext

type HangfireConsoleSerilogEnricher() =
    
    let asyncLocalLogFilter = PerformingContextAccessor() : IPerformingContextAccessor 
    let toLogProperty performingContext =
        (HangfireConsoleSerilogEnricher.LogPropertyName, PerformingContextStructureValue(performingContext))
        |> LogEventProperty

    static member val LogPropertyName = "HangfireJob"
    
    interface ILogEventEnricher with 
        member this.Enrich(logEvent, _) =
            asyncLocalLogFilter.TryGet()
            |> Option.map toLogProperty
            |> Option.iter logEvent.AddOrUpdateProperty

type PCSV = PerformingContextStructureValue

[<RequireQualifiedAccess>]
module HangfireConsoleTextColor =
    
    let ofLogEventLevel = function
    | LogEventLevel.Verbose     -> ConsoleTextColor.Gray 
    | LogEventLevel.Debug       -> ConsoleTextColor.White 
    | LogEventLevel.Information -> ConsoleTextColor.Green 
    | LogEventLevel.Warning     -> ConsoleTextColor.Yellow 
    | LogEventLevel.Error       -> ConsoleTextColor.Red 
    | LogEventLevel.Fatal       -> ConsoleTextColor.Magenta
    | logEventLevel             -> failwithf $"%A{logEventLevel} isn't supported."
    
    let ofLogLevel = function
    | LogLevel.Trace       -> ConsoleTextColor.Gray 
    | LogLevel.Debug       -> ConsoleTextColor.White 
    | LogLevel.Information -> ConsoleTextColor.Green 
    | LogLevel.Warning     -> ConsoleTextColor.Yellow 
    | LogLevel.Error       -> ConsoleTextColor.Red 
    | LogLevel.Critical    -> ConsoleTextColor.Magenta
    | logLevel             -> failwithf $"%A{logLevel} isn't supported."

type HangfireConsoleSink(formatProvider) =
    interface ILogEventSink with
        member this.Emit(logEvent) =
            match logEvent.Properties.TryGetValue(HangfireConsoleSerilogEnricher.LogPropertyName) with
            | true, (:? PCSV as propertyValue) when propertyValue.PerformingContext |> isNull |> not ->
                let hangfireConsoleTextColor = HangfireConsoleTextColor.ofLogEventLevel logEvent.Level
                let message = logEvent.RenderMessage(formatProvider)
                propertyValue.PerformingContext.WriteLine(hangfireConsoleTextColor, message)
            | _  ->
                ()

           
type IHangfireConsoleProgressBarFactory =
    
    abstract TryCreate: name: string * value: double * ConsoleTextColor -> IProgressBar option

[<RequireQualifiedAccess>]
module PerformingContextAccessor =
    
    let writeLine (value: string) color (performingContext: PerformingContext) =
        performingContext.WriteLine(color, value)
    
    let writeProgressBar name value color (performingContext: PerformingContext) =
        performingContext.WriteProgressBar(name, value, color)

type HangfireConsoleProgressBarFactory(performingContextAccessor: IPerformingContextAccessor) =
    
    interface IHangfireConsoleProgressBarFactory with 
        member this.TryCreate(name, value, color) =
            performingContextAccessor.Map(PerformingContextAccessor.writeProgressBar name value color)
    
[<RequireQualifiedAccess>]
module HangfireConsole =
    
    let private getPerformingContext ()           = Option.toObj HangfireSubscriberServerFilter.Value
    let private applyPerformingContext f      = getPerformingContext() |> f
    let private mapMaybePerformingContext mapping = Option.map mapping HangfireSubscriberServerFilter.Value
    let private iterMaybePerformingContext action = Option.iter action HangfireSubscriberServerFilter.Value
    
    let tryCreateProgressBar name value color =
        mapMaybePerformingContext (PerformingContextAccessor.writeProgressBar name value color)
    let createProgressBar name value color =
        applyPerformingContext (PerformingContextAccessor.writeProgressBar name value color)
    let setProgressBarTo (progressBar: IProgressBar) (value: double) = progressBar.SetValue(value)
    
    let tryWriteLine message color =
        mapMaybePerformingContext (PerformingContextAccessor.writeLine message color)
    let writeLine message color =
        applyPerformingContext (PerformingContextAccessor.writeLine message color)
    
    let writeLineWith color prefix message = writeLine $"%s{prefix} %s{message}" color
    let writeTrace       = writeLineWith ConsoleTextColor.Gray "🔍" 
    let writeInformation = writeLineWith ConsoleTextColor.White "ℹ️" 
    let writeSuccess     = writeLineWith ConsoleTextColor.Green "✅" 
    let writeWarning     = writeLineWith ConsoleTextColor.Yellow "⚠️" 
    let writeError       = writeLineWith ConsoleTextColor.Red "❌" 
    let writeFatal       = writeLineWith ConsoleTextColor.Magenta "💥" 
    
    
type HangfireLoggerEmptyScope() =
    
    static member val Instance = new HangfireLoggerEmptyScope()
    interface IDisposable with member this.Dispose() = ()

type PC = PerformingContext
type HangfireLogger(contextAccessor: IPerformingContextAccessor) =
    
    let onPerformingContext consoleTextColor state e (formatMessage: Func<_, _, string>) (performingContext: PC) =
        let message = formatMessage.Invoke(state, e)
        if isNull message then performingContext.WriteLine(consoleTextColor, message)
        else  performingContext.WriteLine(consoleTextColor, message)
    
    interface ILogger with
        member this.BeginScope(_) = HangfireLoggerEmptyScope.Instance
        member this.IsEnabled(_) = true
        member this.Log(logLevel, _, state, e, formatMessage) =
            let consoleTextColor = HangfireConsoleTextColor.ofLogLevel logLevel
            contextAccessor.Iter(onPerformingContext consoleTextColor state e formatMessage)
    
type ICancellationTokenAccessor = abstract member TryGet : unit -> CancellationToken option
    
type ConsoleCancellationTokenAccessor(performingContextAccessor: IPerformingContextAccessor) =
    
    let extractCancellationToken (performingContext: PerformingContext) =
        if isNull performingContext.CancellationToken then CancellationToken.None
        else performingContext.CancellationToken.ShutdownToken
    
    interface ICancellationTokenAccessor with 
        member this.TryGet() = performingContextAccessor.Map(extractCancellationToken)
    
type HangfireConsoleLoggerProvider(performingContextAccessor) =
    interface ILoggerProvider with
        member this.CreateLogger(_) = HangfireLogger(performingContextAccessor)
        member this.Dispose() = ()
        
type IHangfireConsoleJobManager =
    abstract member StartWaitAsync: 
        methodCall: Expression<Func<'Job, Task>> ->
        cancellationToken: CancellationToken ->
        Task<'Result>
            
    abstract member Start: methodCall: Expression<Func<'Job, Task>> -> unit

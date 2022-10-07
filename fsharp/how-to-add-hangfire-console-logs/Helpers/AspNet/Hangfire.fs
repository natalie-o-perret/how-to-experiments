namespace HowToAddHangfireConsoleLogs.Helpers.Hangfire

open System

open Hangfire
open Hangfire.Server
open Hangfire.Common
open Hangfire.Console
open Hangfire.Dashboard
open Hangfire.MemoryStorage

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open HowToAddHangfireConsoleLogs.Helpers.Logging.Hangfire.Implementation


type HangfireTrigger = { CronExpression: string; TimeZone: TimeZoneInfo }

[<RequireQualifiedAccess>]
module HangfireTriggerCron = 
    let fromUtc expression = { CronExpression = expression; TimeZone = TimeZoneInfo.Utc }
    let never() = Cron.Never() |> fromUtc

[<CustomEquality; CustomComparison>]
type HangfireJobDefinition =  
    { Id: string; Job: Job; Trigger: HangfireTrigger }
    member private this.ImportantFields =
        this.Id

    override this.Equals yObj =
        match yObj with
        | :? HangfireJobDefinition as y -> this.ImportantFields = y.ImportantFields
        | _ -> false

    override this.GetHashCode () = hash this.ImportantFields
    interface IComparable with
        member x.CompareTo yObj =
            match yObj with
            | :? HangfireJobDefinition as y -> compare x.ImportantFields y.ImportantFields
            | _ -> invalidArg "yObj" "cannot compare values of different types"

type HangfireDashboardSettings =
    { DarkTheme: bool
      Enabled:   bool }

type HangfireConfig =
    { JobDefinitions: HangfireJobDefinition list
      Dashboard:      HangfireDashboardSettings }

type private DashboardAsyncAuthorization() =
    interface IDashboardAsyncAuthorizationFilter with member _.AuthorizeAsync _ = task { return true }


[<RequireQualifiedAccess>]
module Hangfire =
    
    [<Literal>]
    let private DefaultDashboardPath = "/hangfire"

    let private createInMemoryStorage (globalConfiguration: IGlobalConfiguration) =
        let storageOptions = MemoryStorageOptions()
        storageOptions.FetchNextJobTimeout        <- TimeSpan.FromHours(3.) // default 30min
        storageOptions.JobExpirationCheckInterval <- TimeSpan.FromHours(6.) // default 1h
        let storage = MemoryStorage(storageOptions)
        globalConfiguration.UseStorage(storage) |> ignore
        storage :> JobStorage
    
    let private addOrUpdateRecurringJob (jobManager: IRecurringJobManager) jobDefinition =
        let options = RecurringJobOptions()
        options.TimeZone <- jobDefinition.Trigger.TimeZone
        jobManager.AddOrUpdate(
            jobDefinition.Id,
            jobDefinition.Job,
            jobDefinition.Trigger.CronExpression,
            options)

    let private configure darkMode jobDefinitions (globalConfiguration: IGlobalConfiguration) =
        let automaticRetryAttribute = AutomaticRetryAttribute()
        automaticRetryAttribute.Attempts <- 0
        
        let consoleOptions = ConsoleOptions()
        
        if darkMode then
            consoleOptions.BackgroundColor <- "#333333"
            consoleOptions.TimestampColor  <- "#777777"
            consoleOptions.TextColor       <- "#CCCCCC"

        globalConfiguration
            .UseSerilogLogProvider()
            .UseFilter(automaticRetryAttribute)
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseConsole(consoleOptions)
        |> ignore
        
        if darkMode then globalConfiguration.UseDarkModeSupportForDashboard() |> ignore
        
        let jobManager = createInMemoryStorage globalConfiguration |> RecurringJobManager
        jobDefinitions
        |> Set.iter (addOrUpdateRecurringJob jobManager)

    let private setupBackgroundServer (options: BackgroundJobServerOptions) = options.WorkerCount <- 1
    
    let private requiresPerformingContextAccessor mapping (serviceProvider: IServiceProvider)=
        serviceProvider.GetRequiredService<IPerformingContextAccessor>().Map(mapping)
        |> Option.toObj
    
    let private requiresPerformingContextCancellationToken(serviceProvider: IServiceProvider) = 
        requiresPerformingContextAccessor (fun x -> x.CancellationToken) serviceProvider
    
    let private requiresPerformingContext(serviceProvider: IServiceProvider) = 
        requiresPerformingContextAccessor id serviceProvider
    
    let private addConsoleLoggingSupport (services: IServiceCollection) =
        ignore <| services.AddSingleton<IPerformingContextAccessor, PerformingContextAccessor>
        ignore <| services.AddSingleton<ILoggerProvider, HangfireConsoleLoggerProvider>
        ignore <| services.AddSingleton<ICancellationTokenAccessor, ConsoleCancellationTokenAccessor>
        ignore <| services.AddSingleton<IHangfireConsoleProgressBarFactory, HangfireConsoleProgressBarFactory>
        ignore <| services.AddTransient<IJobCancellationToken>(requiresPerformingContextCancellationToken)
        ignore <| services.AddTransient<PerformingContext>(requiresPerformingContext)
        GlobalJobFilters.Filters.Add(HangfireSubscriberServerFilter())
        services
    
    let addIt darkMode jobDefinitions (services: IServiceCollection) =
        services
            .AddHangfire(configure darkMode jobDefinitions)
            .AddHangfireServer(setupBackgroundServer)
        |> addConsoleLoggingSupport

    let private getDefaultDashboardOptions() =
        let dashboardOptions = DashboardOptions()
        dashboardOptions.AsyncAuthorization   <- [ DashboardAsyncAuthorization() ]
        dashboardOptions.StatsPollingInterval <- int32 (TimeSpan.FromSeconds(5).TotalMilliseconds)
        dashboardOptions
    
    let useIt useDashboard (appBuilder: IApplicationBuilder) =
        if useDashboard then appBuilder.UseHangfireDashboard(DefaultDashboardPath, getDefaultDashboardOptions())
        else appBuilder

[<RequireQualifiedAccess>]
module HowToAddHangfireConsoleLogs.Helpers.Host

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

open Serilog

open HowToAddHangfireConsoleLogs.Helpers.Log
open HowToAddHangfireConsoleLogs.Helpers.Hangfire


let private configureServices hfJobDefs (services: IServiceCollection) =
    services
    |> Hangfire.addIt true hfJobDefs
    |> ignore

let private configureApp (appBuilder: IApplicationBuilder) =
    appBuilder
    |> Hangfire.useIt true
    |> ignore

let private configureWebHost hfJobDefs (builder: IWebHostBuilder) =
    builder
        .UseKestrel()
        .ConfigureServices(configureServices hfJobDefs)
        .Configure(configureApp)
    |> ignore

let buildAndRun hfJobDefs logSettings =
    HostBuilder()
        .UseSerilog(fun _ builder -> Log.applyDefault logSettings builder |> ignore)
        .ConfigureWebHost(configureWebHost hfJobDefs)
        .Build()
        .Run()

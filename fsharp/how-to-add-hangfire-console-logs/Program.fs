namespace WebApp

open Serilog.Events

open HowToAddHangfireConsoleLogs.Jobs
open HowToAddHangfireConsoleLogs.Helpers
open HowToAddHangfireConsoleLogs.Helpers.Log


module Program =

    [<EntryPoint>]
    let main _ =
        let logSettings = { AppName = "My Fantastic App"; MinLogLevel = LogEventLevel.Information; Properties = [] }
        let hfJobDefs = HangfireJobs.getAll()
        
        (hfJobDefs, logSettings)
        ||> Host.buildAndRun
        
        0

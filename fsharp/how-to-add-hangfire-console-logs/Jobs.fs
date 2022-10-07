namespace HowToAddHangfireConsoleLogs.Jobs

open System.ComponentModel

open Hangfire
open Hangfire.Common
open Hangfire.Console

open HowToAddHangfireConsoleLogs.Helpers.Hangfire
open HowToAddHangfireConsoleLogs.Helpers.Logging
open HowToAddHangfireConsoleLogs.Helpers.Logging.Hangfire.Implementation


[<AbstractClass; Sealed>]
type BunchOfRelatedJobs private () =
    static member val Every1stOfMonthAt5amCron = Cron.Monthly(1, 5)
    
    [<DisplayName("My job type description")>]
    static member RunMonthlyStuff() =
        let progressBar1 = HangfireConsole.createProgressBar "hello" 42. ConsoleTextColor.DarkGreen
        Serilog.Log.Logger.Information("test")
        HangfireConsoleLog.verbose     "this is verbose"     []
        HangfireConsoleLog.debug       "this is debug"       []
        HangfireConsoleLog.information "this is information" []
        HangfireConsoleLog.warning     "this is warning"     []
        let progressBar2 = HangfireConsole.createProgressBar "meow" 10. ConsoleTextColor.Red
        HangfireConsoleLog.error       "this is error"       [] 
        HangfireConsoleLog.fatal       "this is fatal"       []
        HangfireConsole.setProgressBarTo progressBar2 100.
        HangfireConsole.writeInformation "niha"
        HangfireConsole.writeTrace "niha"
        HangfireConsole.writeFatal "maxiboom"
        HangfireConsole.writeError "boom"
        HangfireConsole.setProgressBarTo progressBar2 100.
        HangfireConsole.setProgressBarTo progressBar1 10.
    
    [<DisplayName("My job type description 2")>]
    static member RunMonthlyStuff2() =
        let progressBar1 = HangfireConsole.createProgressBar "hello" 42. ConsoleTextColor.DarkGreen
        Serilog.Log.Logger.Information("test")
        HangfireConsoleLog.information "this is qwr"         []
        HangfireConsoleLog.error       "this is error"       [] 
        HangfireConsoleLog.fatal       "this is fatal"       []
        HangfireConsole.setProgressBarTo progressBar1 3.

[<RequireQualifiedAccess>]
module HangfireJobs = 

    let getAll() = set [
         { Id      = "My Hangfire Job Name"
           Job     = Job.FromExpression(fun() -> BunchOfRelatedJobs.RunMonthlyStuff()) 
           Trigger = HangfireTriggerCron.fromUtc BunchOfRelatedJobs.Every1stOfMonthAt5amCron }
         { Id      = "My Hangfire Job Name2"
           Job     = Job.FromExpression(fun() -> BunchOfRelatedJobs.RunMonthlyStuff2()) 
           Trigger = HangfireTriggerCron.fromUtc BunchOfRelatedJobs.Every1stOfMonthAt5amCron }
    ]

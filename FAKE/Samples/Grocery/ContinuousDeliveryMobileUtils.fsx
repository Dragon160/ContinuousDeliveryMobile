module ContinuousDeliveryMobileUtils

    #r @"FAKE.3.5.4/tools/FakeLib.dll"

    #load "ContinuousDeliveryMobile.fsx"

    open Fake
    open System
    open System.IO
    open System.Linq
    open ContinuousDeliveryMobile;

    [<AbstractClass; Sealed>]
    type Utils private () =

        static member IsWindows = Environment.OSVersion.ToString().Contains("Windows")

        static member IsMacOS = not Utils.IsWindows

        static member GetCurrentBuildPlatform = if Utils.IsMacOS then BuildPlatform.OSX else BuildPlatform.Windows

        static member GetNewIncrementedBuildVersion (v:System.Version) =
            new System.Version(v.Major, v.Minor, v.Build + 1)
            
        static member GetNewIncrementedMinorVersion (v:System.Version) =
            new System.Version(v.Major, v.Minor + 1, 0)
            
        static member GetNewIncrementedMajorVersion (v:System.Version) =
            new System.Version(v.Major + 1, 0, 0)  

        static member Exec command args =   
            trace command 
            trace args        
            let result = Shell.Exec(command, args)
            if result <> 0 then failwithf "%s exited with error %d" command result

        static member AsyncExec command args =     
            Shell.AsyncExec(command, args)

        static member RestoreNugetPackages solutionFile pathToNuGetExe =
            Utils.Exec pathToNuGetExe ("restore " + solutionFile)

        static member GetOutputPath projectFile buildConfiguration buildPlatform =
            let path = Path.GetDirectoryName projectFile
            if buildPlatform = "AnyCPU" || buildPlatform = "Any CPU"
            then
                Path.Combine(path, "bin", buildConfiguration)
            else
                Path.Combine(path, "bin", buildPlatform, buildConfiguration)

        static member LaunchAndroidEmulatorIfNeeded emulatorPath adbPath emulatorArgs adbDevicesArgs =
            let adbDevicesResult = 
                ProcessHelper.ExecProcessAndReturnMessages(fun p->
                                p.FileName <- adbPath
                                p.Arguments <- adbDevicesArgs) (System.TimeSpan.FromSeconds 7.1)

            let emulatorRunning = adbDevicesResult.Messages.Exists(fun z -> z.Contains "emulator")
        
            if not emulatorRunning
                then System.Diagnostics.Process.Start(emulatorPath, emulatorArgs) |> ignore
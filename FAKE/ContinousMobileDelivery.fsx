namespace Zuehlke.ContinousMobileDelivery

#r @"FAKE.3.5.4/tools/FakeLib.dll"
#load "Zuehlke.ContinousMobileDelivery.config.fsx"

open Fake
open System
open System.IO
open System.Linq

type Helpers = class

    static member IsMacOS =
        Helpers.IsWindows <> true

    static member IsWindows =
        Environment.OSVersion.ToString().Contains("Windows")

    static member incrementVersion (v:System.Version) =
        if v.Minor < 9
        then
            new System.Version(v.Major, v.Minor, v.Build + 1)
        else if v.Minor < 9
        then
            new System.Version(v.Major, v.Minor + 1, 0)
        else
            new System.Version(v.Major + 1, 0, 0)

    static member Exec (command: string) (args: string) =   
        trace command 
        trace args        
        let result = Shell.Exec(command, args)
        if result <> 0 then failwithf "%s exited with error %d" command result

    static member AsyncExec (command: string) (args: string) =     
        Shell.AsyncExec(command, args)        

    static member RestoreNugetPackages (solutionFile: string) =
        let path = Zuehlke.ContinousMobileDelivery.Config.PathToNuGetExe(Helpers.IsMacOS)
        Helpers.Exec path ("restore " + solutionFile)

    static member RestoreXamarinComponents (solutionFile: string) =
        3 |> ignore
        //solutionFile |> XamarinHelper.RestoreComponents (fun defaults -> {defaults with ToolPath = "tools/xpkg/xamarin-component.exe" })
        
    static member RunNUnitTests (dllPath: string) (xmlPath: string) =
        let path = Zuehlke.ContinousMobileDelivery.Config.PathToNUnitExe(Helpers.IsMacOS)
        Helpers.Exec path (dllPath + " -xml=" + xmlPath)        
        
    // Launch Android Emulator synchronously
    static member LaunchAndroidEmulatorIfNeeded =
        let emulatorPath =
            if Helpers.IsMacOS
                then "/Users/Dev/Library/Developer/Xamarin/android-sdk-mac_x86/tools/emulator"
                else "C:\EclipseADT\sdk\tools\emulator.exe"
    
        let emulatorArgs = "-avd AndroidFast"

        let adbPath = 
            if Helpers.IsMacOS
                then "/Users/Dev/Library/Developer/Xamarin/android-sdk-mac_x86/platform-tools/adb"
                else @"C:\EclipseADT\sdk\platform-tools\adb.exe"

        let adbDevicesArgs = "devices"

        let adbDevicesResult = 
            ProcessHelper.ExecProcessAndReturnMessages(fun p->
                            p.FileName <- adbPath
                            p.Arguments <- adbDevicesArgs) (System.TimeSpan.FromSeconds 7.1)

        let emulatorRunning = adbDevicesResult.Messages.Exists(fun z -> z.Contains "emulator")
        
        if not emulatorRunning
            then System.Diagnostics.Process.Start(emulatorPath, emulatorArgs) |> ignore


    static member RunUITests (uiTestSolution : string) (uiTestProjectFile : string) =

        Helpers.RestoreNugetPackages uiTestSolution   
        Helpers.RestoreXamarinComponents uiTestSolution

        let uiTestProjectFolder = System.IO.FileInfo(uiTestProjectFile).Directory.FullName
        let uiTestBuildFolder = Path.Combine(uiTestProjectFolder, "bin","UITests")
        let uiTestLibraryDll = Path.Combine(uiTestBuildFolder, System.IO.FileInfo(uiTestProjectFile).Name.Replace(".csproj", ".dll"))
        let uiTestLibraryXml = Path.Combine(uiTestBuildFolder, System.IO.FileInfo(uiTestProjectFile).Name.Replace(".csproj", ".testresults.xml"))

        //MSBuild null "Build" [ ("Configuration", "UITests"); ("Platform", "Any CPU") ] [ uiTestProjectFile ] |> ignore

        Helpers.RunNUnitTests uiTestLibraryDll uiTestLibraryXml

    static member Build_iOS (solutionFile: string, ?configuration : string, ?platform : string) =
   
        let buildConfiguration = defaultArg configuration "Debug"
        let buildPlatform = defaultArg platform "iPhoneSimulator"
            
        Helpers.RestoreNugetPackages solutionFile        
        Helpers.RestoreXamarinComponents solutionFile
        
        if Helpers.IsMacOS
            then 
                XamarinHelper.iOSBuild (fun defaults ->
                {defaults with
                    ProjectPath = solutionFile
                    Configuration = buildConfiguration + "|" + buildPlatform
                    Target = "Build"
                    MDToolPath = defaults.MDToolPath
                })
            else               
               MSBuild null "Build" [ ("Configuration", buildConfiguration); ("Platform",  buildPlatform)] [solutionFile] |> ignore        

    static member Build_Android (solutionFile: string, projectFile: string, ?configuration : string, ?platform : string) =
        
        let buildConfiguration = defaultArg configuration "Debug"
        let buildPlatform = defaultArg platform "Any CPU"
        
        Helpers.RestoreNugetPackages solutionFile        
        Helpers.RestoreXamarinComponents solutionFile
        
        let outputPath = Path.Combine((new System.IO.FileInfo(projectFile)).Directory.FullName , "bin" , buildConfiguration)                 
        MSBuild null "Rebuild" [ ("Configuration", buildConfiguration); ("Platform", buildPlatform) ] [ solutionFile ] |> ignore                    
        XamarinHelper.AndroidPackage (fun defaults ->
                {defaults with
                    ProjectPath = projectFile
                    Configuration = buildConfiguration
                    OutputPath = outputPath
                })|>ignore
end



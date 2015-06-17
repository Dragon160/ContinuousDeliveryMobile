module ContinuousDeliveryMobileCore

#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
#load "ContinuousDeliveryMobileUtils.fsx"
#load "Configuration.fsx"

open Fake;
open System.Linq;
open System.IO;
open ContinuousDeliveryMobile;
open ContinuousDeliveryMobileUtils;

let configuration = 
    (Configuration.Configuration() :> IConfiguration)

let technicalConfiguration = 
    (Configuration.Configuration() :> ITechnicalConfiguration)

let buildApp (app:App) =
    let projectOrSolution = match app.BuildObject with
                                | Project value -> value
                                | Solution value -> value
        
    trace ("building " + projectOrSolution + " in " + app.BuildConfiguration.Configuration)
        
    if app.TargetPlatform = IOS
    then 
        XamarinHelper.iOSBuild(fun defaults ->
            {defaults with
                ProjectPath = projectOrSolution
                Configuration = app.BuildConfiguration.Configuration + "|" + app.BuildConfiguration.Platform
                Target = "Build"
                MDToolPath = defaults.MDToolPath
            })
    elif app.TargetPlatform = Android
    then
        MSBuild null "Rebuild" [ ("Configuration", app.BuildConfiguration.Configuration); ("Platform", app.BuildConfiguration.Platform) ] [ projectOrSolution ] |> ignore                    
        let outputPath = System.IO.Path.Combine((new System.IO.FileInfo(app.ProjectFile)).Directory.FullName , "bin" , app.BuildConfiguration.Configuration)  
        XamarinHelper.AndroidPackage (fun defaults ->
                {defaults with
                    ProjectPath = app.ProjectFile
                    Configuration = app.BuildConfiguration.Configuration
                    OutputPath = outputPath
                })|>ignore

let build (platform:TargetPlatform) =
    let apps = configuration.Build 
               |> Array.filter(fun a -> a.TargetPlatform = platform)

    if Array.isEmpty apps then trace ("No apps are configured to build for the platform " + platform.ToString()) 

    for app in apps do
        buildApp app

let runUnitTests() = 
    let runTest (dll:System.String) (testResultTargetFile) =
        System.Console.WriteLine ("##### RUNNING TESTS: " + dll)

        let nUnitPathDictionary = technicalConfiguration.NUnitPath
        let nUnitPath = nUnitPathDictionary.[Utils.GetCurrentBuildPlatform]

        Utils.Exec
            nUnitPath
            (dll + " -xml=" + testResultTargetFile)

    for unitTest in configuration.UnitTest do

        let solutionFile = unitTest.SolutionFile
        let buildConfiguration = unitTest.BuildConfiguration
        let buildTargetFile = unitTest.BuildTargetFile
        let testResultTargetFile = unitTest.TestResultTargetFile

        MSBuild null "Clean" [ ("Configuration", buildConfiguration.Configuration); ("Platform", buildConfiguration.Platform) ] [ solutionFile ] |> ignore
        MSBuild null "Build" [ ("Configuration", buildConfiguration.Configuration); ("Platform", buildConfiguration.Platform) ] [ solutionFile ] |> ignore

        buildTargetFile
        |> runTest testResultTargetFile
        |> ignore


let package (platform:TargetPlatform) =
    let packages = configuration.Package
                   |> Array.filter(fun (p) -> p.App.TargetPlatform = platform)

    if Array.isEmpty packages then trace ("No apps are configured to packaging for the platform " + platform.ToString()) 

    for package in packages do
        
        buildApp package.App
        let outputPath = ContinuousDeliveryMobileUtils.Utils.GetOutputPath package.App.ProjectFile package.App.BuildConfiguration.Configuration package.App.BuildConfiguration.Platform
        
        if package.App.TargetPlatform = IOS
        then
            let ipaFile = System.IO.Directory.EnumerateFiles(outputPath, "*.ipa").SingleOrDefault()
            if System.String.IsNullOrEmpty(ipaFile) <> true
            then System.IO.File.Move(ipaFile, package.TargetFile)
            else System.Console.WriteLine "Could not build Ad-Hoc iOS IPA file"
        elif package.App.TargetPlatform = Android
        then
            let apkFiles = Directory.EnumerateFiles(outputPath, "*.apk").ToArray();
            let keyStoreParams = technicalConfiguration.AndroidKeystoreParams(package.App)

            let move (file:FileInfo) =
                let target = new FileInfo (Path.Combine(Path.GetDirectoryName(package.TargetFile), file.Name))
                System.IO.File.Move(file.FullName, target.FullName)
                target

            let signAlign (file:FileInfo) =
                file |> XamarinHelper.AndroidSignAndAlign (fun defaults ->
                        {defaults with
                            KeystorePath = keyStoreParams.KeystorePath
                            KeystoreAlias = keyStoreParams.KeystoreAlias
                            KeystorePassword = keyStoreParams.KeystorePassword
                        })

            if apkFiles.Length > 0
                then
                    for f in apkFiles do
                        new FileInfo(f) |> move |> signAlign |> ignore

            else
                System.Console.WriteLine "Could not build Android APK file"

type DefaultTargetImplementations() =
    interface ITargetImplementations with
    
        member this.build platform = build platform
        member this.package platform = package platform
        member this.runUnitTests() = runUnitTests()
    
    end

let targetImplementations = DefaultTargetImplementations() |> technicalConfiguration.GetTargetImplementations

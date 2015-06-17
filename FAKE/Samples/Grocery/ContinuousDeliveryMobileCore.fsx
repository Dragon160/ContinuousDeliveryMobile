module ContinuousDeliveryMobileCore

#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
#load "ContinuousDeliveryMobileUtils.fsx"
#load "Configuration.fsx"

open Fake;
open System.Linq;
open ContinuousDeliveryMobile;
open ContinuousDeliveryMobileUtils;

let configuration = 
    (Configuration.Configuration() :> IConfiguration)

let technicalConfiguration =
    (Configuration.Configuration() :> ITechnicalConfiguration)

let build (platform:TargetPlatform) =
    let apps = configuration.Build 
               |> Array.filter(fun a -> a.TargetPlatform = platform)

    if Array.isEmpty apps then trace ("No apps are configured to build for the platform " + platform.ToString()) 

    for app in apps do
        let projectOrSolution = match app.BuildObject with
                                | Project value -> value
                                | Solution value -> value
        
        trace ("building " + projectOrSolution + " in " + app.BuildConfiguration.Configuration)
        
        if platform = IOS
        then 
            XamarinHelper.iOSBuild(fun defaults ->
                {defaults with
                    ProjectPath = projectOrSolution
                    Configuration = app.BuildConfiguration.Configuration + "|" + app.BuildConfiguration.Platform
                    Target = "Build"
                    MDToolPath = defaults.MDToolPath
                })
        else
            MSBuild null "Rebuild" [ ("Configuration", app.BuildConfiguration.Configuration); ("Platform", app.BuildConfiguration.Platform) ] [ projectOrSolution ] |> ignore                    
            let outputPath = System.IO.Path.Combine((new System.IO.FileInfo(app.ProjectFile)).Directory.FullName , "bin" , app.BuildConfiguration.Configuration)  
            XamarinHelper.AndroidPackage (fun defaults ->
                    {defaults with
                        ProjectPath = app.ProjectFile
                        Configuration = app.BuildConfiguration.Configuration
                        OutputPath = outputPath
                    })|>ignore

let runUnitTests = 
    for unitTest in configuration.UnitTest do

        let buildObject = unitTest.BuildObject
        let buildConfiguration = unitTest.BuildConfiguration
        let targetDll = unitTest.TargetDll
        let targetResultFile = unitTest.TargetResultFile

        MSBuild null "Clean" [ ("Configuration", "Test"); ("Platform", "Any CPU") ] [ buildObject ] |> ignore
        MSBuild null "Build" [ ("Configuration", "Test"); ("Platform", "Any CPU") ] [ buildObject ] |> ignore

        let srcFolder = (new System.IO.FileInfo(buildObject)).DirectoryName

        let testDlls = System.IO.Directory.GetFiles(srcFolder, "*.Tests.dll", System.IO.SearchOption.AllDirectories)

        let moveResults (xml:System.String) =
        let resultsFolder = "testresults"
        if System.IO.Directory.Exists (resultsFolder) <> true
            then System.IO.Directory.CreateDirectory(resultsFolder) |> ignore
        System.IO.File.Copy(xml, System.IO.Path.Combine(resultsFolder, (new System.IO.FileInfo(xml)).Name), true)

        let runTest (dll:System.String) =
            System.Console.WriteLine ("##### RUNNING TESTS: " + dll)
            let xmlPath = dll.Replace(".Tests.dll", ".Tests.testresults.xml")

            let nUnitPath = technicalConfiguration.NUnitPath

            Utils.Exec
                nUnitPath
                (dll + " -xml=" + xmlPath)
     
            xml     

        for d in testDlls.Where(dlls) do
            d  |> runTest  |> moveResults |> ignore




let package (platform:TargetPlatform) =
    let packages = configuration.Package
                   |> Array.filter(fun (p) -> p.App.TargetPlatform = platform)

    if Array.isEmpty packages then trace ("No apps are configured to packaging for the platform " + platform.ToString()) 

    for package in packages do

    3 |> ignore

type DefaultTargetImplementations() =
    interface ITargetImplementations with
    
        member this.build platform = build platform
    
    end

let targetImplementations =
    DefaultTargetImplementations() |> (Configuration.Configuration() :> ITechnicalConfiguration).GetTargetImplementations
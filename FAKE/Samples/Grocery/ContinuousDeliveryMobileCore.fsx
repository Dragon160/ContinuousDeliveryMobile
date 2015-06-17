module ContinuousDeliveryMobileCore

#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
#load "Configuration.fsx"

open Fake;
open ContinuousDeliveryMobile;

let configuration = 
    (Configuration.Configuration() :> IConfiguration)

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
    else
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


let package (platform:TargetPlatform) =
    let packages = configuration.Package
                   |> Array.filter(fun (p) -> p.App.TargetPlatform = platform)

    if Array.isEmpty packages then trace ("No apps are configured to packaging for the platform " + platform.ToString()) 

    for package in packages do
        //buildApp package.App.
//     Helpers.Build_iOS(Slideflight.iOS_SolutionFile, "Ad-Hoc", "iPhone")
//    let buildPath = Path.Combine(Slideflight.iOS_AudienceAppProjectPath, "bin", "iPhone", "Ad-Hoc")
//    let ipaFile = Directory.EnumerateFiles(buildPath, "*.ipa").SingleOrDefault()
//    if System.String.IsNullOrEmpty(ipaFile) <> true
//        then System.IO.File.Move(ipaFile, Path.Combine(Slideflight.publishPath("*.ipa"), Path.GetFileNameWithoutExtension(ipaFile) + "_Ad-Hoc" + Path.GetExtension(ipaFile)))
//    else
//        System.Console.WriteLine "Could not build Ad-Hoc iOS IPA file"

    3 |> ignore

type DefaultTargetImplementations() =
    interface ITargetImplementations with
    
        member this.build platform = build platform
    
    end

let targetImplementations =
    DefaultTargetImplementations() |> (Configuration.Configuration() :> ITechnicalConfiguration).GetTargetImplementations
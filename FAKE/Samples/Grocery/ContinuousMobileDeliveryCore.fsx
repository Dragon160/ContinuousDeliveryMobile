module ContinuousMobileDeliveryCore

#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousMobileDelivery.fsx"
#load "Configuration.fsx"

open Fake;
open ContinuousMobileDelivery;

let configuration = 
    (Configuration.Configuration() :> IConfiguration)

let build (platform:TargetPlatform) =
    let apps = configuration.Build 
               |> Array.filter(fun (p,_,_,_) -> p = platform)
    for (_, buildObject, projectFile, (buildConfiguration, buildPlatform)) in apps do
        let projectOrSolution = match buildObject with
                                | Project value -> value
                                | Solution value -> value
        
        System.Console.WriteLine ("building " + projectOrSolution + " in " + buildConfiguration)
        
        if platform = IOS
        then 
            XamarinHelper.iOSBuild(fun defaults ->
                {defaults with
                    ProjectPath = projectOrSolution
                    Configuration = buildConfiguration + "|" + buildPlatform
                    Target = "Build"
                    MDToolPath = defaults.MDToolPath
                })
        else
            MSBuild null "Rebuild" [ ("Configuration", buildConfiguration); ("Platform", buildPlatform) ] [ projectOrSolution ] |> ignore                    
            let outputPath = System.IO.Path.Combine((new System.IO.FileInfo(projectFile)).Directory.FullName , "bin" , buildConfiguration)  
            XamarinHelper.AndroidPackage (fun defaults ->
                    {defaults with
                        ProjectPath = projectFile
                        Configuration = buildConfiguration
                        OutputPath = outputPath
                    })|>ignore


type DefaultTargetImplementations() =
    interface ITargetImplementations with
    
        member this.build platform = build platform
    
    end

let targetImplementations =
    DefaultTargetImplementations() |> (Configuration.Configuration() :> ITechnicalConfiguration).GetTargetImplementations
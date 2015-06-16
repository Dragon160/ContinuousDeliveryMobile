#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "Config.fsx"
#load "ContinousMobileDelivery.fsx"

open Fake;
open System.Linq;

Target "build" (fun () ->
    Run "build-ios"
    Run "build-android"
    Run "unit-test"
)

Target "test" (fun () ->
    Run "unit-test"
    Run "test-ios"
    Run "test-android"
)

Target "prepare-package"(fun() ->
    Run "prepare-package-ios"
    Run "prepare-package-android"
    Run "prepare-package-ppt"
)

Target "package" (fun () ->
    if Zuehlke.ContinousMobileDelivery.Helpers.IsWindows
    then 
        Run "package-ppt"
    else    
        Run "package-ios"
        Run "package-android"
)

let filter (x:System.String) = 
    x.Length > 3

Target "unit-test" (fun () ->
    3 |> ignore
//   Slideflight.runUnitTests(fun d -> d.Contains(System.IO.Path.Combine("bin", "Debug")) && (d.Contains("PowerPoint") <> true) && (d.Contains("Client.Tests") <> true))
)

RunTarget() 
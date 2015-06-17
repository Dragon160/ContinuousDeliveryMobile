#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousMobileDelivery.fsx"
#load "Configuration.fsx"
#load "ContinuousMobileDeliveryCore.fsx"

open Fake;
open ContinuousMobileDelivery;
open ContinuousMobileDeliveryCore;


Target "build" (fun () ->
    Run "build-ios"
    Run "build-android"
    Run "unit-test"
)

Target "build-ios" (fun () ->
    targetImplementations.build IOS
)

Target "build-android" (fun () ->
    targetImplementations.build Android
)

Target "unit-test" (fun () ->
    3 |> ignore
)

Target "test" (fun () ->
    Run "unit-test"
    Run "test-ios"
    Run "test-android"
)

RunTarget()

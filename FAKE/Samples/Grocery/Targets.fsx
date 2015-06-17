#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
#load "Configuration.fsx"
#load "ContinuousDeliveryMobileCore.fsx"

open Fake;
open ContinuousDeliveryMobile;
open ContinuousDeliveryMobileCore;


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
    targetImplementations.runUnitTests
)

Target "ui-test" (fun () ->
    Run "ui-test-ios"
    Run "ui-test-android"
)

Target "ui-test-ios" (fun () ->
    3 |> ignore
)

Target "ui-test-android" (fun () ->
    3 |> ignore
)

Target "test" (fun () ->
    Run "unit-test"
    Run "ui-ios"
)

Target "package" (fun () ->
    Run "package-ios"
    Run "package-android"
)
Target "package-ios" (fun () ->
    targetImplementations.package IOS
)
Target "package-android" (fun () ->
    targetImplementations.package Android
)


RunTarget()

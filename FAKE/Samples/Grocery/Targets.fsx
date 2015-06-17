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

Target "test" (fun () ->
    Run "unit-test"
    Run "test-ios"
    Run "test-android"
)

RunTarget()

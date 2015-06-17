#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
#load "Configuration.fsx"

open Fake;

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
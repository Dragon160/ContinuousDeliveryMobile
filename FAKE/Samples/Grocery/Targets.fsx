#r @"FAKE.3.5.4/tools/FakeLib.dll"

#load "ContinuousDeliveryMobile.fsx"
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
    build IOS
)

Target "build-android" (fun () ->
    build Android
)

Target "unit-test" (fun () ->
    build Android
)

Target "test" (fun () ->
    Run "unit-test"
    Run "test-ios"
    Run "test-android"
)

RunTarget()

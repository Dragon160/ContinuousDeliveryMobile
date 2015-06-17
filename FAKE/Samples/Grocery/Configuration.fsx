#load "ContinuousMobileDelivery.fsx"

open ContinuousMobileDelivery

type Grocery() =

    let androidApp config = (Android, Project "Grocery.Android.csproj", config)

    let iOSApp config = (IOS, Project "Grocery.IOS.csproj", config)

    let unitTests = (Project "Grocery.UnitTests.csproj", "Test", "Grocery.UnitTests.dll") // dll name kann evtl aus Projektdatei gelesen werden

    let uiTests = (Project "Grocery.UITests.csproj", "UITest", "Grocery.UITests.dll")

    interface IConfiguration with

        member this.Build = [| 
            androidApp "Debug"; 
            iOSApp "Debug" 
            |]

        member this.UnitTest = [| 
            (unitTests, "testresults.xml") 
            |]

        member this.UITest = [| 
            (androidApp "Release", uiTests, "uitestresults.xml") ;
            |]

        member this.Package = Skip

        member this.Publish = Skip


    interface ITechnicalConfiguration with

        member this.EmulatorPath = [|
                (Windows, @"C:\EclipseADT\sdk\tools\emulator.exe");
                (OSX, "/Users/Dev/Library/Developer/Xamarin/android-sdk-mac_x86/tools/emulator")
            |]

        member this.AdbPath = [|
                (Windows, @"C:\EclipseADT\sdk\platform-tools\adb.exe");
                (OSX, "/Users/Dev/Library/Developer/Xamarin/android-sdk-mac_x86/platform-tools/adb")
            |]
module Configuration

#load "ContinuousMobileDelivery.fsx"
open ContinuousMobileDelivery

    [<Sealed>]
    type Configuration () =
            let solution = System.IO.Path.Combine("GroceryShopper", "GroceryShopper", "GroceryShopper.sln")
            let project path = System.IO.Path.Combine("GroceryShopper", "GroceryShopper", "GroceryShopper.Forms", path, path + ".csproj")
            
            let androidApp config = (Android, Solution solution, project "GroceryShopper.Forms.Droid", (config, "Any CPU"))

            let iOSApp config = (IOS, Solution solution, project "GroceryShopper.Forms.iOS", (config, "iPhoneSimulator")) // use iPhone for release

            let unitTests = (Project "Grocery.UnitTests.csproj", ("Test", "Any CPU"), "Grocery.UnitTests.dll") // dll name kann evtl aus Projektdatei gelesen werden

            let uiTests = (Project "Grocery.UITests.csproj", ("UITest", "Any CPU"), "Grocery.UITests.dll")

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

                member this.GetTargetImplementations defaultTargetImplementations = defaultTargetImplementations
module Configuration

#load "ContinuousMobileDelivery.fsx"
open ContinuousMobileDelivery

    [<Sealed>]
    type Configuration () =
            let solution = System.IO.Path.Combine("GroceryShopper", "GroceryShopper", "GroceryShopper.sln")
            let project path = System.IO.Path.Combine("GroceryShopper", "GroceryShopper", path, path + ".csproj")
            
            let androidApp config = (Android, Solution solution, project "GroceryShopper.Droid", config, "Any CPU")

            let iOSApp config = (IOS, Solution solution, project "", config, "iPhoneSimulator") // use iPhone for release

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

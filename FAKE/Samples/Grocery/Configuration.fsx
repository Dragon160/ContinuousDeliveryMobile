#load "../ContinuousMobileDelivery.fsx"
open ContinuousMobileDelivery

type Grocery() =

    let androidApp config = (Android, Project "Grocery.Android.csproj", config)
    let iOSApp config = (IOS, Project "Grocery.IOS.csproj", config)
    let unitTests = (Project "Grocery.UnitTests.csproj", "Test", "Grocery.UnitTests.dll")
    let uiTests = (Project "Grocery.UITests.csproj", "UITest", "Grocery.UITests.dll")

    interface IConfiguration with
        member this.Build = [| androidApp "Debug"; iOSApp "Debug" |]
        member this.UnitTest = [| (unitTests, "testresults.xml") |]
        member this.UITest = [| (androidApp "Release", uiTests, "uitestresults.xml") |]
        member this.Package = Skip
        member this.Publish = Skip



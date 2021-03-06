module ContinuousDeliveryMobile

    open System.Collections.Generic;

    type TargetPlatform =
        | Android
        | IOS
        | WindowsPhone
        
    type SolutionFile = string
    type ProjectFile = string

    type BuildPlatform =
        | Windows
        | OSX
        
    type BuildConfiguration(configuration:string, platform:string)=
        member this.Configuration = configuration  
        member this.Platform = platform  

    type TargetFile = string
    
    type App(targetPlatform : TargetPlatform, solutionFile : SolutionFile, projectFile : ProjectFile, buildConfiguration : BuildConfiguration) = 
        member this.TargetPlatform = targetPlatform  
        member this.SolutionFile = solutionFile
        member this.ProjectFile = projectFile
        member this.BuildConfiguration = buildConfiguration

    type UnitTest(solutionFile : SolutionFile, buildConfiguration : BuildConfiguration, buildTargetFile : TargetFile, testResultTargetFile : TargetFile) = 
        member this.SolutionFile = solutionFile
        member this.BuildConfiguration = buildConfiguration
        member this.BuildTargetFile = buildTargetFile
        member this.TestResultTargetFile = testResultTargetFile

    type UITest(app : App, unitTest : UnitTest) = 
        member this.App = app
        member this.UnitTest = unitTest

    type Package(app : App, targetFile : TargetFile) = 
        member this.App = app
        member this.TargetFile = targetFile

    type ITargetImplementations =
        interface
            abstract member build: TargetPlatform -> unit
            abstract member runUnitTests: unit -> unit
            abstract member runUITests: TargetPlatform -> unit
            abstract member package: TargetPlatform -> unit
        end

    type IConfiguration =
        interface
            abstract member Build : App[]
            abstract member UnitTest : UnitTest[]
            abstract member UITest : UITest[]
            abstract member Package : Package[]
            abstract member Publish : string[]
        end

    type AndroidKeystoreParams(keystorePath:string,keystoreAlias:string,keystorePassword:string)=
        member this.KeystorePath = keystorePath
        member this.KeystoreAlias = keystoreAlias
        member this.KeystorePassword = keystorePassword

    type ITechnicalConfiguration  =
        interface
            abstract member AndroidKeystoreParams : App -> AndroidKeystoreParams
            abstract member EmulatorPath : Dictionary<BuildPlatform, string>
            abstract member AdbPath: Dictionary<BuildPlatform, string>
            abstract member NUnitPath: Dictionary<BuildPlatform, string>
            abstract member GetTargetImplementations: ITargetImplementations -> ITargetImplementations
        end
    
    let Skip = [||]


module ContinuousMobileDelivery

    type Platform =
        | Android
        | IOS
        
    type SolutionFile = string
    type ProjectFile = string

    type BuildPlatform =
        | Windows
        | OSX

    type BuildObject = 
        | Solution of SolutionFile
        | Project of ProjectFile
        
    type BuildConfiguration = string
    type BuildPlatform = string


    type TargetFile = string
    type App = (Platform * BuildObject * ProjectFile * BuildConfiguration * BuildPlatform)
    type UnitTest = (BuildObject * BuildConfiguration * TargetFile)

    type IConfiguration =
        interface
            abstract member Build : App[]
            abstract member UnitTest : (UnitTest*TargetFile)[]
            abstract member UITest : (App*UnitTest*TargetFile)[]
            abstract member Package : (App*TargetFile)[]
            abstract member Publish : string[]
        end

    type ITechnicalConfiguration  =
        interface
            abstract member EmulatorPath : (BuildPlatform * string)[]
            abstract member AdbPath: (BuildPlatform * string)[]
        end
    
    let Skip = [||]


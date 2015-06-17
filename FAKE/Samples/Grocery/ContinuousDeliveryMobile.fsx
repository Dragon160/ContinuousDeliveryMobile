module ContinuousDeliveryMobile

    type TargetPlatform =
        | Android
        | IOS
        | WindowsPhone
        
    type SolutionFile = string
    type ProjectFile = string

    type BuildPlatform =
        | Windows
        | OSX

    type BuildObject = 
        | Solution of SolutionFile
        | Project of ProjectFile
        
    type BuildConfiguration = (string*string)

    type TargetFile = string

    type App = (TargetPlatform * BuildObject * ProjectFile * BuildConfiguration)

    type UnitTest = (BuildObject * BuildConfiguration * TargetFile)

    type ITargetImplementations =
        interface
            abstract member build: TargetPlatform -> unit
        end

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
            abstract member GetTargetImplementations: ITargetImplementations -> ITargetImplementations
        end
    
    let Skip = [||]


module Zuehlke.ContinousMobileDelivery.Config

open System
open System.IO
open System.Linq

let PathToNuGetExe isOnMac = Path.Combine(Directory.GetCurrentDirectory(),  "..", "Tools", "NuGet", "NuGet.exe")

let PathToNUnitExe isOnMac = 
    if isOnMac
        then "/Library/Frameworks/Mono.framework/Versions/Current/bin/nunit-console4" 
        else Path.Combine(Directory.GetCurrentDirectory(), "..", "Tools", "NUnit-2.6.4", "bin", "nunit-console.exe")




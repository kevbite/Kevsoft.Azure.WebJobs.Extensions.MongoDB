var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = "./Kevsoft.Azure.WebJobs.Extensions.MongoDB.sln";
var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";

Task("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreCleanSettings
    {
        Configuration = configuration
    };
    
    DotNetCoreClean(sln, settings);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(sln, new DotNetCoreRestoreSettings());
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild(sln, new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        Logger = "trx",
    };

    var projectFiles = GetFiles("./test/**/*.csproj");
    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath, settings);
    }
});

Task("Pack-Library")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
     var settings = new DotNetCorePackSettings
     {
        Configuration = configuration,
        OutputDirectory = "./artifacts/",
        NoBuild = true,
        ArgumentCustomization = args => args.Append("/p:Version=" + version)
     };

     DotNetCorePack(sln, settings);
});


Task("Upload-Artifacts")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Pack-Library")
    .Does(() =>
{
    foreach (var filePath in GetFiles("./artifacts/*")) 
    { 
        AppVeyor.UploadArtifact(filePath, new AppVeyorUploadArtifactsSettings
        {
            DeploymentName = filePath.GetFilename().ToString()
        });
    }
});


Task("Upload-Test-Results")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    foreach (var filePath in GetFiles("./test/**/TestResults/*.trx")) 
    { 
        AppVeyor.UploadTestResults(filePath, AppVeyorTestResultsType.MSTest);
    }
});

Task("Default")
    .IsDependentOn("Upload-Artifacts")
    .IsDependentOn("Upload-Test-Results");

RunTarget(target);

var target = Argument("target", "Report");

#addin nuget:?package=Cake.Coverlet&version=2.1.2
#tool nuget:?package=ReportGenerator&version=4.0.4



/*  Specify the relative paths to your tests projects here. */
var testProjectsRelativePaths = new string[]
{
    "../tests/CodeCoverageCalculation.Common.Tests/CodeCoverageCalculation.Common.Tests.csproj",
    "../tests/CodeCoverageCalculation.Domain.Tests/CodeCoverageCalculation.Domain.Tests.csproj"
};

/*  Change the output artifacts and their configuration here. */
var parentDirectory = Directory("..");
var coverageDirectory = parentDirectory + Directory("code_coverage");
var testProjectDirectory = @$"{parentDirectory}/source/productcatalog/test/";
var cuberturaFileName = "results";
var cuberturaFileExtension = ".cobertura.xml";
var reportTypes = "HTML;HTMLSummary"; // Use "Html" value locally for performance and files' size.
var coverageFilePath = coverageDirectory + File(cuberturaFileName + cuberturaFileExtension);
var jsonFilePath = coverageDirectory + File(cuberturaFileName + ".json");

Task("Clean")
    .Does(() =>
{
    if (!DirectoryExists(coverageDirectory))
        CreateDirectory(coverageDirectory);
    else
        CleanDirectory(coverageDirectory);
});

Task("Test")
    .IsDependentOn("Clean")
    .Does(() =>
{
    

    var testSettings = new DotNetCoreTestSettings
    {
        // 'trx' files will be used to publish the results of tests' execution in an Azure DevOps pipeline.
        ArgumentCustomization = args => args.Append($"--logger trx")
    };

    var coverletSettings = new CoverletSettings
    {
        CollectCoverage = true,
        CoverletOutputDirectory = coverageDirectory,
        CoverletOutputName = cuberturaFileName,
        CoverletOutputFormat = CoverletOutputFormat.cobertura,
        MergeWithFile = jsonFilePath
    };

    var testProjects = GetFiles($@"{testProjectDirectory}/**/*.csproj");
    
    foreach (var testProject in testProjects)
    {
        Information(testProject);
        DotNetCoreTest(testProject, testSettings, coverletSettings);
    }
});

Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    // var reportSettings = new ReportGeneratorSettings
    // {
    //     ArgumentCustomization = args => args.Append($"-reportTypes:{reportTypes}")
    // };
    // ReportGenerator(coverageFilePath, coverageDirectory, reportSettings);
});

RunTarget(target);
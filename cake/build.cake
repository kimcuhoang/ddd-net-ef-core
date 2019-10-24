
var target = Argument("target", "Report");
var configuration = Argument("Configuration", "Release");

#addin nuget:?package=Cake.Coverlet&version=2.3.4
#tool nuget:?package=ReportGenerator&version=4.3.2


/*  Change the output artifacts and their configuration here. */
var parentDirectory = Directory("..");
var testProjectDirectory = Directory($"{parentDirectory}/source/productcatalog/test");
var coverageDirectory = Directory($"{parentDirectory}/code_coverage");
var cuberturaFileName = "results";
var cuberturaFileExtension = ".cobertura.xml";
var reportTypes = "HTML;HTMLSummary"; // Use "Html" value locally for performance and files' size.
var coverageFilePath = coverageDirectory + File(cuberturaFileName + cuberturaFileExtension);

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
    var testResultDir = MakeAbsolute(coverageDirectory);
    var testSettings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => 
            args.Append($"--logger trx")
                .Append($"--results-directory {testResultDir}")
    };

    var coverletSettings = new CoverletSettings
    {
        CollectCoverage = true,
        CoverletOutputName = cuberturaFileName,
        CoverletOutputFormat = CoverletOutputFormat.cobertura
    };

    
    var testProjects = GetFiles($@"{testProjectDirectory}/**/*.csproj");
    
    foreach (var testProject in testProjects)
    {
        var filename = testProject.GetFilenameWithoutExtension().FullPath;
        coverletSettings.CoverletOutputName = File($"{filename}.{cuberturaFileName}{cuberturaFileExtension}");
        coverletSettings.CoverletOutputDirectory = Directory($"{coverageDirectory}"); 

        DotNetCoreTest(testProject, testSettings, coverletSettings);
    }
});

Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    var reportSettings = new ReportGeneratorSettings
    {
        ReportTypes = new List<ReportGeneratorReportType> {ReportGeneratorReportType.Html, ReportGeneratorReportType.HtmlSummary},
        Verbosity = ReportGeneratorVerbosity.Verbose,
    };
    ReportGenerator($"{Directory($"{coverageDirectory}")}/*.{cuberturaFileName}{cuberturaFileExtension}", coverageDirectory, reportSettings);
});

RunTarget(target);
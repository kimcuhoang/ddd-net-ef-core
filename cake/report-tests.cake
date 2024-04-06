
var target = Argument("target", "Report");
var configuration = Argument("Configuration", "Release");

#addin nuget:?package=Cake.Coverlet&version=4.0.1
#tool nuget:?package=ReportGenerator&version=5.2.4


/*  Change the output artifacts and their configuration here. */
var parentDirectory = Directory("..");
var testProjectDirectory = Directory($"{parentDirectory}/source/Services/product-catalog/DDD.ProductCatalog.Tests");
var coverageDirectory = Directory($"{parentDirectory}/code_coverage");
var cuberturaFileName = "results";
var cuberturaFileExtension = ".cobertura.xml";
var reportTypes = "HTML;HTMLSummary"; // Use "Html" value locally for performance and files' size.
var coverageFilePath = coverageDirectory + File(cuberturaFileName + cuberturaFileExtension);

Task("Clean").Does(() =>
{
    if (DirectoryExists(coverageDirectory))
    {
        DeleteDirectory(coverageDirectory, new DeleteDirectorySettings
        {
            Recursive = true,
            Force = true
        });
    }

    CreateDirectory(coverageDirectory);
});

Task("Test")
.IsDependentOn("Clean")
.Does(() =>
{
    var testResultDir = MakeAbsolute(coverageDirectory);
    var testSettings = new DotNetTestSettings
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

        DotNetTest(testProject, testSettings, coverletSettings);
    }
});

Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    var reportSettings = new ReportGeneratorSettings
    {
        ReportTypes = new List<ReportGeneratorReportType> { ReportGeneratorReportType.Html, ReportGeneratorReportType.HtmlSummary },
        Verbosity = ReportGeneratorVerbosity.Verbose,
    };

    var reportFiles = GetFiles($"{Directory($"{coverageDirectory}")}/*.{cuberturaFileName}{cuberturaFileExtension}");

    ReportGenerator(reportFiles, coverageDirectory, reportSettings);
});

RunTarget(target);
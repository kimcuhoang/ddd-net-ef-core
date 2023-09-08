///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var workingSpace = @"../";
var tyeFolder = $@"{workingSpace}.tye";
var project = @"source/Services/product-catalog/DDD.ProductCatalog.Infrastructure";
var startupProject = @"source/Services/product-catalog/DDD.ProductCatalog.WebApi";
var dbContext = "ProductCatalogDbContext";

var outputEfBundle = @"./cake/product-catalog-bundle.exe";

var connectionString = @"Server=localhost,1433;Database=DotNETDDD;User Id=sa;Password=P@ssw0rd!;Encrypt=False;MultipleActiveResultSets=True";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////



Task("Update-Database").Does(() => 
{
   var argumentsBuilder = new ProcessArgumentBuilder();
        argumentsBuilder
            .Append("ef database update")
            .Append($"-p {project}")
            .Append($"-s {startupProject}")
            .Append($"-c {dbContext}");

        var processSettings = new ProcessSettings {
            Arguments = argumentsBuilder,
            WorkingDirectory = workingSpace,
            NoWorkingDirectory = false
        };
        
   StartProcess("dotnet", processSettings);
});

Task("Generate-EfCore-Migration-Bundle").Does(() => 
{
   var argumentsBuilder = new ProcessArgumentBuilder();
        argumentsBuilder
            .Append("ef migrations bundle")
            .Append("--runtime win-x64")
            .Append($"-p {project}")
            .Append($"-s {startupProject}")
            .Append($"-o {outputEfBundle}")
            .Append("--force");

        var processSettings = new ProcessSettings {
            Arguments = argumentsBuilder,
            WorkingDirectory = workingSpace,
            NoWorkingDirectory = false
        };
        
   StartProcess("dotnet", processSettings);
});

Task("Apply-Migration-Bundle").Does(() => 
{
   var argumentsBuilder = new ProcessArgumentBuilder()
         .Append($"--connection \"{connectionString}\"");

   var processSettings = new ProcessSettings {
            Arguments = argumentsBuilder,
            WorkingDirectory = workingSpace,
            NoWorkingDirectory = false
        };

   var pathOfWorkingSpace = new DirectoryPath(workingSpace);

   var pathOfBundleFile = pathOfWorkingSpace.CombineWithFilePath(outputEfBundle);

   StartProcess(pathOfBundleFile, processSettings);
});

Task("Tye")
.IsDependentOn("Generate-EfCore-Migration-Bundle")
.IsDependentOn("Apply-Migration-Bundle")
.Does(() =>
{
   var logDirectory = System.IO.Path.Combine(tyeFolder, @".logs");
   var tyeFilePath = System.IO.Path.Combine(tyeFolder, "tye.yaml");

   if (DirectoryExists(logDirectory))
   {
      DeleteDirectory(logDirectory, new DeleteDirectorySettings
      {
         Recursive = true,
         Force = true
      });
   }

   StartProcess("tye", new ProcessSettings
   {
      Arguments = new ProcessArgumentBuilder()
           .Append("run")
           .Append(tyeFilePath)
           // .Append("--watch")
           .Append("--dashboard")
   }
   );
}); 


Task("Default")
.Does(() =>
{
   Information("Hello Cake!");
});

RunTarget(target);
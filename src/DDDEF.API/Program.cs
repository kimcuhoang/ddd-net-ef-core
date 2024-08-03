using DDDEF.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.BuildWebApplication().Construct();

await app.RunAsync();

public partial class Program { }
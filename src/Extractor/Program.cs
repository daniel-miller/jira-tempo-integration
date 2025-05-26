using System.Globalization;

using Extractor;

using Microsoft.Extensions.Configuration;

using Serilog;

// Step 1. Load application settings and configure logging to write to the console.

var path = AppContext.BaseDirectory;

var builder = new ConfigurationBuilder()
    .SetBasePath(path)
    .AddJsonFile("appsettings.json", false);

var configuration = builder.Build();

var section = configuration.GetRequiredSection("Extractor");

var settings = section.Get<ExtractorSettings>();

if (settings == null)
{
    Log.Error("Configuration settings cannot be null.");
    return;
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Step 2. Validate the command line.

if (args.Length < 2)
{
    Log.Error("Two date arguments are required in the format yyyy-MM-dd.");
    return;
}

if (!DateTime.TryParseExact(args[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var since) ||
    !DateTime.TryParseExact(args[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var until))
{
    Log.Error("Invalid date format. Use yyyy-MM-dd.");
    return;
}

// Step 3. Run the application.

Log.Information("Extracting worklog time entries between {since:MMM d, yyyy} and {until:MMM d, yyyy} (inclusive).", since, until);

settings.Since = since;

settings.Until = until;

var app = new ExtractorApp(settings);

await app.Run();
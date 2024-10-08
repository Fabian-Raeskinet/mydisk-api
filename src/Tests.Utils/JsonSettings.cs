using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace MyDisks.Tests.Utils;

public class JsonSettings
{
    public static readonly JsonSettings XUnit = new("xunit.json");
    private readonly IConfigurationRoot _configurationRoot;

    private JsonSettings(string path)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(GetExecutingAssemblyDirectory()!)
            .AddJsonFile(path, true, true);

        _configurationRoot = builder.Build();
    }

    private static string? GetExecutingAssemblyDirectory()
    {
        var location = Assembly.GetCallingAssembly().Location;
        var uri = new Uri(location);
        return Path.GetDirectoryName(uri.LocalPath);
    }

    public string? GetConnectionString(string connectionString)
    {
        return _configurationRoot.GetConnectionString(connectionString);
    }
}
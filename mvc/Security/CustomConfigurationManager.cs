using System.IO;
using Microsoft.Extensions.Configuration;

namespace mvc.Security.Keys
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.Keys.json")
                    .Build();
        }
    }
}
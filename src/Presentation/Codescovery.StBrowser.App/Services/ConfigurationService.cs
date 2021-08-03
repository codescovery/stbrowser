using Codescovery.StBrowser.App.Models;
using Microsoft.Extensions.Configuration;

namespace Codescovery.StBrowser.App.Services
{
    public  class ConfigurationService
    {
        private readonly IConfigurationRoot _configuration;
        public Configuration Configuration{ get; set; }
        public ConfigurationService()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
            Configuration = MapConfiguration();
        }

        private Configuration MapConfiguration()
        {
            return _configuration.Get<Configuration>();
        }
    }
}

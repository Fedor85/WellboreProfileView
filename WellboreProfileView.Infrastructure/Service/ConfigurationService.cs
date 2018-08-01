using System.Configuration;
using WellboreProfileView.Interfaces.Services;

namespace WellboreProfileView.Infrastructure.Service
{
    public class ConfigurationService : IConfigurationService
    {
        public string ConnectionString { get; set; }

        public ConfigurationService()
        {
            string connectionName = ConfigurationManager.AppSettings["DefaultConnection"];
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Ground
{
    public static class Configurations
    {
        static string _DocumentationConnectionString;
        public static string DocumentationConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DocumentationConnectionString))
                    _DocumentationConnectionString = GetConnectionString("DocumentationConnectionString");
                return _DocumentationConnectionString;
            }
        }

        public static string GetConnectionString(string appSettingsKey)
        {
            var cs = ConfigurationManager.ConnectionStrings[appSettingsKey];
            if (cs != null)
                return cs.ConnectionString;

            return null;
        }
    }
}

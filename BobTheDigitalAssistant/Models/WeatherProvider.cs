using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace BobTheDigitalAssistant.Models
{
    public class WeatherProvider
    {
        public int WeatherProviderID { get; set; }
        public string Name { get; set; }
        public WeatherProviderAccessTypes AccessType { get; set; }

        /// <summary>
        /// The list of urls that the weather provider needs to use
        /// </summary>
        public List<string> urls = new List<string>();

        public WeatherProvider() : this(-1, "", WeatherProviderAccessTypes.EXTERNAL_URL)
        {
        }

        public WeatherProvider(int WeatherProviderID, string Name, WeatherProviderAccessTypes AccessType)
        {
            this.WeatherProviderID = WeatherProviderID;
            this.Name = Name;
            this.AccessType = AccessType;
        }

        public static WeatherProvider FromDataRow(SqliteDataReader reader)
        {
            try
            {
                string serviceName = reader["weatherProviderName"].ToString();
                string baseURL = reader["baseURL"].ToString();
                string[] urlParts = reader["urlParts"].ToString().Split("###");
                List<string> providerURLs = new List<string>();
                WeatherProviderAccessTypes type = (WeatherProviderAccessTypes)Enum.Parse(typeof(WeatherProviderAccessTypes), reader["type"].ToString());
                int weatherProviderID = int.Parse(reader["weatherProviderID"].ToString());
                // for each url part, combine it with the base url
                urlParts.Reverse();
                foreach (string part in urlParts)
                {
                    providerURLs.Add(baseURL + part);
                }
                var createdProvider = new WeatherProvider()
                {
                    WeatherProviderID = weatherProviderID,
                    Name = serviceName,
                    urls = providerURLs,
                    AccessType = type
                };
                return createdProvider;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        public enum WeatherProviderAccessTypes
        {
            EXTERNAL_URL = 1, // accessible through browser
            APP = 2, // accessible through app installed on machine
            CURL = 3 // accessible through an api that doesn't require the browser
        }
    }
}
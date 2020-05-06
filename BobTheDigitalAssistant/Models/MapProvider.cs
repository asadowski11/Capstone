using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Capstone.Models
{
    public class MapProvider
    {
        public int MapProviderID { get; set; }
        public string Name { get; set; }
        public string BaseURL { get; set; }
        public MapProviderAccessTypes AccessType { get; set; }

        /// <summary>
        /// Parts of the url (such as url queries). Each part will be used to build the final url to make the request to the map API with
        /// </summary>
        public Dictionary<string, string> URLParts { get; } = new Dictionary<string, string>();

        public MapProvider() : this(-1, "", "", MapProviderAccessTypes.EXTERNAL_URL)
        {
        }

        public MapProvider(int MapProviderID, string Name, string BaseURL, MapProviderAccessTypes AccessType)
        {
            this.MapProviderID = MapProviderID;
            this.Name = Name;
            this.BaseURL = BaseURL;
            this.AccessType = AccessType;
        }

        public static MapProvider FromDataRow(SqliteDataReader reader)
        {
            try
            {
                string serviceName = reader["mapProviderName"].ToString();
                string baseURL = reader["baseURL"].ToString();
                string[] urlParts = reader["urlParts"].ToString().Split("###");
                string providerURL = baseURL;
                MapProviderAccessTypes type = (MapProviderAccessTypes)Enum.Parse(typeof(MapProviderAccessTypes), reader["type"].ToString());
                int mapProviderID = int.Parse(reader["mapProviderID"].ToString());
                // for each url part, combine it with the base url
                foreach (string part in urlParts.Reverse())
                {
                    providerURL += part;
                }
                var createdProvider = new MapProvider()
                {
                    MapProviderID = mapProviderID,
                    Name = serviceName,
                    BaseURL = providerURL,
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

        public enum MapProviderAccessTypes
        {
            EXTERNAL_URL = 1, // accessible through browser
            APP = 2, // accessible through app installed on machine
        }
    }
}
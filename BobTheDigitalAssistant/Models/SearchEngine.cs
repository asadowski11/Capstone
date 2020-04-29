using Microsoft.Data.Sqlite;

namespace Capstone.Models
{
    public class SearchEngine
    {
        public int SearchEngineID { get; set; }
        public string Name { get; set; }
        public string BaseURL { get; set; }
        public string QueryString { get; set; }

        public SearchEngine() : this(-1, "", "", "")
        {

        }

        public SearchEngine(int SearchEngineID, string Name, string BaseURL, string QueryString)
        {
            this.SearchEngineID = SearchEngineID;
            this.Name = Name;
            this.BaseURL = BaseURL;
            this.QueryString = QueryString;
        }

        public static SearchEngine FromDataRow(SqliteDataReader reader)
        {
            SearchEngine createdSearchEngine = new SearchEngine(int.Parse(reader["searchEngineID"].ToString()), reader["searchEngineName"].ToString(),
               reader["searchEngineBaseURL"].ToString(), reader["searchEngineQueryString"].ToString());
            return createdSearchEngine;
        }
    }
}

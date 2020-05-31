using Microsoft.Data.Sqlite;

namespace BobTheDigitalAssistant.Models
{
	public class SearchableWebsite
	{
		public int SearchableWebsiteID { get; set; }
		public string Name { get; set; }
		public string BaseURL { get; set; }
		public string QueryString { get; set; }
		public string SpaceReplacement { get; set; }

		public SearchableWebsite() : this(-1, "", "", "", "")
		{
		}

		public SearchableWebsite(int SearchableWebsiteID, string Name, string BaseURL, string QueryString, string SpaceReplacement)
		{
			this.SearchableWebsiteID = SearchableWebsiteID;
			this.Name = Name;
			this.BaseURL = BaseURL;
			this.QueryString = QueryString;
			this.SpaceReplacement = SpaceReplacement;
		}

		public static SearchableWebsite FromDataRow(SqliteDataReader reader)
		{
			SearchableWebsite createdSearchableWebsite = new SearchableWebsite(int.Parse(reader["searchableWebsitesID"].ToString()), reader["searchableWebsiteName"].ToString(),
			   reader["searchableWebsiteBaseURL"].ToString(), reader["searchableWebsiteQueryString"].ToString(), reader["spaceReplacement"].ToString());
			return createdSearchableWebsite;
		}
	}
}

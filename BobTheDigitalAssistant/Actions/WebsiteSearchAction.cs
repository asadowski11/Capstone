using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using BobTheDigitalAssistant.Common;
using BobTheDigitalAssistant.Models;
using Windows.UI.Xaml.Controls;

namespace BobTheDigitalAssistant.Actions
{
    public class WebsiteSearchAction : Action
    {
        private SearchableWebsite desiredSearchableWebsite { get; set; }
        private SearchEngine desiredSearchEngine { get; set; }

        public WebsiteSearchAction(string CommandString)
        {
            this.CommandString = CommandString;
        }

        public override void PerformAction()
        {
            //get list of searchable websites from database
            List<SearchableWebsite> allSearchableWebsites = StoredProcedures.QueryAllSearchableWebsites();
            //need to check if user provided a website to search
            bool isUserProvidedWebsiteSearch = false;
            //go through list of searchable websites. return true if user included the searchable website in search
            //this will also set the website if there is a match
            isUserProvidedWebsiteSearch = GetActionFromCommand(allSearchableWebsites);
            string searchParameters;
            string searchQuery;

            if (isUserProvidedWebsiteSearch)
            {
                //find what is wanted to be searched and concatenate with + for end of url
                searchParameters = GetSearchParameters(isUserProvidedWebsiteSearch);
                searchQuery = BuildSearchQuery(desiredSearchableWebsite, searchParameters);
                //launch browser. this will be done with the default browser
                LaunchSearch(searchQuery);
            }
            else
            {
                //sets desiredSearchEngine, which is the default selected in settings
                GetDefaultSearchEngine();
                searchParameters = GetSearchParameters(isUserProvidedWebsiteSearch);
                searchQuery = BuildSearchQuery(desiredSearchEngine, searchParameters);
                //launch browser. this will be done with the default browser
                LaunchSearch(searchQuery);
            }
            // show a link to the search
            this.ClearArea();
            var linkElement = new HyperlinkButton();
            linkElement.Content = $"{searchParameters} on {(desiredSearchableWebsite != null ? desiredSearchableWebsite.Name : desiredSearchEngine?.Name)}";
            linkElement.NavigateUri = new Uri(searchQuery);
            linkElement.FontSize = 24;
            RelativePanel.SetAlignHorizontalCenterWithPanel(linkElement, true);
            RelativePanel.SetAlignVerticalCenterWithPanel(linkElement, true);
            this.DynamicArea.Children.Add(linkElement);
            // announce to the user that we're searching for something
            TextToSpeechEngine.SpeakText(this.MediaElement, $"Sure, searching for {linkElement.Content}");
        }

        private void GetDefaultSearchEngine()
        {
            Setting preferredSearchEngineSetting = StoredProcedures.QuerySettingByName("Search Engine");
            string preferredSearchEngineName = preferredSearchEngineSetting.GetSelectedOption().DisplayName;
            // after this, use the name to query info from the search engine table
            desiredSearchEngine = StoredProcedures.QuerySearchEngineByName(preferredSearchEngineName);
        }

        private bool GetActionFromCommand(List<SearchableWebsite> allSearchableWebsites)
        {
            foreach (SearchableWebsite searchableWebsite in allSearchableWebsites)
            {
                var websiteRegex = new Regex($"(?i){searchableWebsite.Name}(?-i)");
                if (websiteRegex.IsMatch(this.CommandString))
                {
                    this.desiredSearchableWebsite = searchableWebsite;
                    return true;
                }
            }
            return false;
        }

        //put together base url, query search, and the search parameters
        private string BuildSearchQuery(SearchableWebsite websiteSearch, string toSearch)
        {
            string searchQuery = websiteSearch.BaseURL + websiteSearch.QueryString + WebUtility.UrlEncode(toSearch).Replace("+", websiteSearch.SpaceReplacement);

            return searchQuery;
        }

        private string BuildSearchQuery(SearchEngine searchEngine, string toSearch)
        {
            string searchQuery = searchEngine.BaseURL + searchEngine.QueryString + WebUtility.UrlEncode(toSearch);

            return searchQuery;
        }

        private async void LaunchSearch(string search)
        {
            // Create a Uri object from a URI string
            var uri = new System.Uri(search);

            //launch uri with search
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        //find what is wanted to be searched
        private string GetSearchParameters(bool isSearchableWebsite)
        {
            string searchParameters = "";

            try
            {
                var searchRegex = new Regex("(?i) for |search(?!= for)(?-i)");
                var splitParams = searchRegex.Split(this.CommandString);
                searchParameters = splitParams[splitParams.Length - 1].Trim();
                if (isSearchableWebsite)
                {
                    Regex removeSearchWords = new Regex($"(?i) (in|at|on)? ?{this.desiredSearchableWebsite.Name}(?-i)");
                    searchParameters = removeSearchWords.Replace(searchParameters, string.Empty);
                }
                else
                {
                    Regex removeSearchWords = new Regex($"(?i)(in|at|on)? ?{this.desiredSearchEngine.Name}(?-i)");
                    searchParameters = removeSearchWords.Replace(searchParameters, string.Empty);
                }
            }
            catch (Exception)
            {
            }

            return searchParameters.Trim();
        }
    }
}
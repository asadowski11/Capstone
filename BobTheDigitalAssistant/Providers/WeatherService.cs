using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capstone.Common;
using Capstone.Models;
using Capstone.Models.json.NationalWeatherService;
using Newtonsoft.Json;
using Windows.Web.Http;

namespace Capstone.Providers
{
    public static class WeatherService
    {
        public static async Task<List<WeatherInfo>> GetWeather()
        {
            // TODO get preferred weather provider once settings are implemented. For the scope of the capstone, the only weather provider is the national weather service
            WeatherProvider weatherProvider = StoredProcedures.QueryWeatherProvider("National Weather Service");
            if (weatherProvider.Name.Equals("National Weather Service"))
            {
                // TODO after we get more weather providers, split this out into its own method to prevent polluting a common space
                List<ForecastPeriod> nwsPeriods = await GetWeatherFromNWS(weatherProvider);
                // the list of weather infos to return
                List<WeatherInfo> weatherInfos = new List<WeatherInfo>();
                if (nwsPeriods != null)
                {
                    // convert the periods to weather infos and add those infos to the list
                    nwsPeriods.ForEach(period =>
                    {
                        WeatherInfo weatherInfo = new WeatherInfo();
                        weatherInfo.CurrentTemperature = period.temperature;
                        // national weather service doesn't give high or low, so ignore those
                        weatherInfo.WindSpeed = period.windSpeed;
                        weatherInfo.Description = period.detailedForecast;
                        // fix the name up so the date time parser can parse it out
                        period.name = DateTimeParser.ReplaceDaytimeNamesWithTimeValue(period.name);
                        weatherInfo.DateApplicable = DateTimeParser.ParseDateTimeFromText(period.name);
                        // precipitation chance and humidity are not given by the nws api in separate fields
                        weatherInfos.Add(weatherInfo);
                    });
                }
                return weatherInfos;
            }
            return null;
        }

        private static async Task<List<ForecastPeriod>> GetWeatherFromNWS(WeatherProvider provider)
        {
            // get the office and grid points
            string getGridPointsURL = provider.urls.Find(url => url.Contains("/points/:latitude,:longitude"));
            string forecastURL = provider.urls.Find(url => url.Contains("/forecast"));
            // get the location's latitude and longitude
            Dictionary<string, double> coordinates = await LocationProvider.GetLatitudeAndLongitude();
            if (coordinates != null)
            {
                double latitude = coordinates["latitude"];
                double longitude = coordinates["longitude"];
                // fill in the url's values
                getGridPointsURL = getGridPointsURL.Replace(":latitude", latitude.ToString()).Replace(":longitude", longitude.ToString());
                Point GridPointResponse = JsonConvert.DeserializeObject<Point>(await MakeSimpleGetRequest(getGridPointsURL));
                // get the office, gridX, and gridY
                PointProperties properties = GridPointResponse.properties;
                string office = properties.office;
                string gridX = properties.gridX.ToString();
                string gridY = properties.gridY.ToString();
                // replace the parts of the forecast url with our values
                forecastURL = forecastURL.Replace(":office", office).Replace(":gridX", gridX).Replace(":gridY", gridY);
                Forecast ForecastResponse = JsonConvert.DeserializeObject<Forecast>(await MakeSimpleGetRequest(forecastURL));
                return ForecastResponse.properties.periods;
            }
            else
            {
                return null;
            }
        }

        private static async Task<string> MakeSimpleGetRequest(string url)
        {
            HttpClient httpClient = new HttpClient();
            // the request will require a user agent string
            var headers = httpClient.DefaultRequestHeaders;
            string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(userAgent))
            {
                throw new Exception("Invalid User Agent Header: " + userAgent);
            }
            // the uri to the resource
            Uri requestUri = new Uri(url);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string responseBody = null;
            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                responseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to make request: " + e.Message);
            }
            return responseBody;
        }
    }
}
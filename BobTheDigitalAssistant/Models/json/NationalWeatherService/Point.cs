using System.ComponentModel;
using Newtonsoft.Json;

namespace Capstone.Models.json.NationalWeatherService
{
    [DisplayName("Point")]
    [Description("Represents a point obtained from the NWS points endpoint")]
    public class Point
    {
        [JsonProperty]
        public string id;

        [JsonProperty]
        public string type;

        [JsonProperty]
        public object geometry;

        [JsonProperty(propertyName: "properties")]
        public PointProperties properties;
    }

    public class PointProperties
    {
        [JsonProperty(propertyName: "cwa")]
        public string office;

        [JsonProperty(propertyName: "forecastOffice")]
        public string forecastOfficeURL;

        [JsonProperty]
        public int gridX;

        [JsonProperty]
        public int gridY;

        [JsonProperty]
        public string forecast;

        [JsonProperty]
        public string forecastHourly;

        [JsonProperty]
        public string forecastGridData;

        [JsonProperty]
        public string observationStations;

        [JsonProperty]
        public object relativeLocation;

        [JsonProperty]
        public string forecastZone;

        [JsonProperty]
        public string county;

        [JsonProperty]
        public string fireWeatherZone;

        [JsonProperty]
        public string timeZone;

        [JsonProperty]
        public string radarStation;
    }
}
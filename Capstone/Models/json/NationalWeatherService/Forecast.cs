using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models.json.NationalWeatherService
{
    public class Forecast
    {
        [JsonProperty]
        public string type;
        [JsonProperty]
        public object geometry;
        [JsonProperty]
        public ForecastProperties properties;
    }

    public class ForecastProperties
    {
        [JsonProperty]
        public DateTime updated;
        [JsonProperty]
        public string units;
        [JsonProperty]
        public string forecastGenerator;
        [JsonProperty]
        public DateTime generatedAt;
        [JsonProperty]
        public DateTime updatedTime;
        [JsonProperty]
        public string validTimes;
        [JsonProperty]
        public object elevation;
        [JsonProperty]
        public List<ForecastPeriod> periods;
    }

    public class ForecastPeriod
    {
        [JsonProperty]
        public int number;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public DateTime startTime;
        [JsonProperty]
        public DateTime endTime;
        [JsonProperty]
        public bool isDaytime;
        [JsonProperty]
        public float temperature;
        [JsonProperty]
        public string temperatureunit;
        [JsonProperty]
        public string temperatureTrend;
        [JsonProperty]
        public string windSpeed;
        [JsonProperty]
        public string windDirection;
        [JsonProperty(propertyName: "icon")]
        public string iconURL;
        [JsonProperty]
        public string shortForecast;
        [JsonProperty]
        public string detailedForecast;
    }
}

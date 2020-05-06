using System;

namespace BobTheDigitalAssistant.Models
{
    /// <summary>
    /// Contains info for a single bit of weather, including temperature, humidity, wind, and precipitation
    /// </summary>
    public class WeatherInfo
    {
        public float CurrentTemperature { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public string WindSpeed { get; set; }
        public float Humidity { get; set; }

        /// <summary>
        /// 1-2 word description of the weather, like "sunny" or "cloudy"
        /// </summary>
        public string Description { get; set; }

        public float PrecipitationChance { get; set; }
        public DateTime DateApplicable { get; set; }

        public WeatherInfo() : this(0.0f, 0.0f, 0.0f, "", 0.0f, "", 0.0f)
        {
        }

        public WeatherInfo(float CurrentTemperature, float High, float Low, string WindSpeed, float Humidity, string Description, float PrecipitationChance)
        {
            this.CurrentTemperature = CurrentTemperature;
            this.High = High;
            this.Low = Low;
            this.WindSpeed = WindSpeed;
            this.Humidity = Humidity;
            this.Description = Description;
            this.PrecipitationChance = PrecipitationChance;
        }
    }
}
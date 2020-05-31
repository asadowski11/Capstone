using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace BobTheDigitalAssistant.Providers
{
	internal class LocationProvider
	{
		private static async Task<GeolocationAccessStatus> RequestLocationAccess()
		{
			return await Geolocator.RequestAccessAsync();
		}

		/// <summary>
		/// Gets a raw geoposition object and returns it
		/// </summary>
		/// <returns>a raw geoposition object with a bunch of information</returns>
		/// <exception cref="LocationProviderException">If there's an error getting location (such as if the user disabled location access)</exception>
		public static async Task<Geoposition> GetCurrentLocation()
		{
			GeolocationAccessStatus accessStatus = await RequestLocationAccess();
			switch (accessStatus)
			{
				case GeolocationAccessStatus.Allowed:
					var locator = new Geolocator();
					var position = await locator.GetGeopositionAsync();
					return position;

				case GeolocationAccessStatus.Unspecified:
					throw new LocationProviderException("Unspecified Geolocation status");
				case GeolocationAccessStatus.Denied:
					throw new LocationProviderException("Location access is denied by the user");
				default:
					throw new LocationProviderException("Unknown error (hit default branch in switch)");
			}
		}

		/// <summary>
		/// Gets the latitude and longitude of the user's current location and returns it in a dictionary. If there's an error getting the user's location, null is returned instead.
		/// </summary>
		/// <returns>a dictionary with the user's latitude and longitude, or null if there was an error retrieving location information</returns>
		public static async Task<Dictionary<string, double>> GetLatitudeAndLongitude()
		{
			Dictionary<string, double> coords = new Dictionary<string, double>();
			try
			{
				Geoposition geoposition = await GetCurrentLocation();
				coords.Add("latitude", geoposition.Coordinate.Point.Position.Latitude);
				coords.Add("longitude", geoposition.Coordinate.Point.Position.Longitude);
			}
			catch (LocationProviderException)
			{
				// indicate an error to the caller
				coords = null;
			}
			catch (NullReferenceException)
			{
				coords = null;
			}
			return coords;
		}
	}

	public sealed class LocationProviderException : Exception
	{
		public LocationProviderException()
		{
		}

		public LocationProviderException(string message) : base(message)
		{
		}
	}
}

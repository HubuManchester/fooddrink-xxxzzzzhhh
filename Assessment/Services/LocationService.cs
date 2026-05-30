namespace Assessment.Services
{
    public class LocationService
    {
        private const double MockLatitude = 39.9042;
        private const double MockLongitude = 116.4074;
        private const string MockAddress = "Dongcheng District, Beijing";

#pragma warning disable CA1822
        public async Task<(double Latitude, double Longitude, string Address)?> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        return GetMockLocation("Location permission denied, using simulated location");
                    }
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(1));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location is null)
                {
                    return GetMockLocation("GPS signal weak, using simulated location");
                }

                string address;
                try
                {
                    var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var firstPlacemark = placemarks?.FirstOrDefault();
                    address = firstPlacemark is not null
                        ? $"{firstPlacemark.Locality ?? ""} {firstPlacemark.Thoroughfare ?? ""} {firstPlacemark.SubThoroughfare ?? ""}".Trim()
                        : "Unknown address";
                }
                catch
                {
                    address = $"{location.Latitude:F4}, {location.Longitude:F4}";
                }

                return (location.Latitude, location.Longitude, address);
            }
            catch (Exception)
            {
                return GetMockLocation("Location service unavailable, using simulated location");
            }
        }

        private static (double Latitude, double Longitude, string Address)? GetMockLocation(string reason)
        {
            System.Diagnostics.Debug.WriteLine($"[Location] {reason}: {MockLatitude}, {MockLongitude}");
            return (MockLatitude, MockLongitude, $"{MockAddress} ({reason})");
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
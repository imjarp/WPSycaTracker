using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SycaBackgroundTask
{
    public sealed class GeoPhoneContextProvider
    {
        public static int GetSignalStrength()
        {
            IReadOnlyList<Windows.Networking.Connectivity.ConnectionProfile> profiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();

            var match = profiles.Where(p => p.IsWwanConnectionProfile == true).FirstOrDefault();

            if (match == null)
                return 0;

            return match.GetSignalBars() == null ? 0 : (int)match.GetSignalBars();

        }

        public static int GetBatteryLevel()
        {
            return Windows.Phone.Devices.Power.Battery.GetDefault().RemainingChargePercent;
        }

        public static bool IsDataConnectionEnable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        public static bool IsLocationServicesDISABLE() 
        {
            var geolocator = new Windows.Devices.Geolocation.Geolocator();

            return geolocator.LocationStatus == Windows.Devices.Geolocation.PositionStatus.Disabled;
             
        }
    }
}

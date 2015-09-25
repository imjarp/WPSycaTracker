using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.UI.Notifications;

namespace SycaBackgroundTask
{
    public sealed class GeoTimerContextBackgroundTask : IBackgroundTask
    {
        internal static Geolocator _Geolocator;

        static readonly string TASK_NAME = "GeoTimerBackgroundsk";

        const string title = "Syca Tracker";

        void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral backgroundDeferral = taskInstance.GetDeferral();

            System.Diagnostics.Debug.WriteLine("Empezando");


            try
            {

                InitializeIfNeeded();

                var phoneContext = GetPartialPhoneContext();

                if (!phoneContext.LocationServicesEnable) 
                {
                    InsertPhoneContext(phoneContext);
                    backgroundDeferral.Complete();
                }

                this.GetPositionUpdate(backgroundDeferral);

                var empezandoMessage = string.Format("Empezando {0}", DateTime.Now);

                ShowToast(title, empezandoMessage);


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                backgroundDeferral.Complete();

            }

            finally
            {

                var terminandoMessage = string.Format("Terminando {0}", DateTime.Now);
                ShowToast(title, terminandoMessage);

            }


        }

        private void InsertPhoneContext(GeoLocationRepository.GeoLocationItem phoneContext)
        {
            GeoLocationRepository.DatabaseHelper dbHelper = new GeoLocationRepository.DatabaseHelper();
            dbHelper.Insert(phoneContext);
        }

        private static GeoLocationRepository.GeoLocationItem GetPartialPhoneContext()
        {
            return new GeoLocationRepository.GeoLocationItem ()
            {
                BatteryLevel = GeoPhoneContextProvider.GetBatteryLevel(),
                DataConnectionEnable = GeoPhoneContextProvider.IsDataConnectionEnable(),
                SignalStrength = GeoPhoneContextProvider.GetSignalStrength(),
                EventDate = DateTime.Now,
                LocationServicesEnable = !GeoPhoneContextProvider.IsLocationServicesDISABLE(),
                SentServer = false
            };
        }

        void _Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private void InitializeIfNeeded()
        {
            if (_Geolocator == null)
            {
                _Geolocator = new Geolocator();

                _Geolocator.DesiredAccuracy = PositionAccuracy.Default;

                _Geolocator.DesiredAccuracyInMeters = 100;

                _Geolocator.ReportInterval = 5 * 1000;

                //_Geolocator.PositionChanged += _Geolocator_PositionChanged;
            }
        }

        private static void ShowToast(string title, string text)
        {
            ToastTemplateType toastType = ToastTemplateType.ToastText02;

            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastType);
            XmlNodeList toastTextElement = toastXml.GetElementsByTagName("text");
            toastTextElement[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElement[1].AppendChild(toastXml.CreateTextNode(text));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }

        private async void GetPositionUpdate(BackgroundTaskDeferral backgroundDeferral)
        {

            var result = await _Geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5),
                                                                 timeout: TimeSpan.FromMinutes(1)
                                                                );

            var phoneContext = GetPartialPhoneContext();

            string latLong = string.Format("date = {0} lat = {1}, long = {2}", DateTime.Now.ToString(), result.Coordinate.Point.Position.Latitude, result.Coordinate.Point.Position.Longitude);

            if(result!=null && result.Coordinate!=null)
            {
                phoneContext.LocationServicesEnable = true;
                phoneContext.Latitude = result.Coordinate.Point.Position.Latitude;
                phoneContext.Longitude = result.Coordinate.Point.Position.Longitude;
            }

            InsertPhoneContext(phoneContext);

            ShowToast(title, latLong);

            backgroundDeferral.Complete();

        }

        public static async void RegisterAsync()
        {

            await InternalRegisterAsync();

        }

        public static bool IsTaskRegistered
        {
            get
            {
                var hasTask = BackgroundTaskRegistration.AllTasks.Any(reg =>
                  reg.Value.Name == TASK_NAME);

                return (hasTask);
            }
        }

        static async Task InternalRegisterAsync()
        {

            await RegisterTaskAsync();
        }

        static async Task RegisterTaskAsync()
        {
            await BackgroundExecutionManager.RequestAccessAsync();
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = TASK_NAME;
            builder.SetTrigger(new TimeTrigger(15, false));
            builder.TaskEntryPoint = typeof(GeoTimerContextBackgroundTask).FullName;
            builder.Register();
        }




        /*
         * 
             IReadOnlyList<ConnectionProfile> profiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();
             foreach (ConnectionProfile profile in profiles)
             {
                 if (profile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                 Debug.WriteLine("{0} {1} {2} bars", profile.ProfileName, profile.GetNetworkConnectivityLevel(), profile.GetSignalBars());
             }
         * 
         */

        /*
          * Geolocator geolocator = new Geolocator();

             if (geolocator.LocationStatus == PositionStatus.Disabled)
             {
                 ...
             }
          */

        /*
         *  public string GetBatteryPercentage()
            {
                 return Windows.Phone.Devices.Power.Battery.GetDefault().RemainingChargePercent.ToString();
            }
         */

        /*
          * public static bool checkConnection()
     {
         return Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
     }

     public static int typeConnection()
     {
         switch (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType)
         {
             default:
                 return 0;
             case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                 return 1;
             case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                 return 1;
             case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                 return 2;
         }
     }*/


    }
}

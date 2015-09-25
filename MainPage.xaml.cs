using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Collections.ObjectModel;
using SycaBackgroundTask;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SycaTracker
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            
            this.DataContext = this;


        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.


            if (!GeoTimerContextBackgroundTask.IsTaskRegistered)
            {
                SycaBackgroundTask.GeoTimerContextBackgroundTask.RegisterAsync();
            }


            checkServices();

            try
            {
                GeoLocationRepository.DatabaseHelper dbHelper = new GeoLocationRepository.DatabaseHelper();

                dbHelper.OnCreate();

                var result  = dbHelper.GetAll();
            }
            catch(Exception er)
            {
                var error = er.Message;
            }

            
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            var message = new Windows.UI.Popups.MessageDialog("Tu me hiciste click");

            await message.ShowAsync();

            ShowToast();

        }

        private void checkServices()
        {

            var battery = SycaBackgroundTask.GeoPhoneContextProvider.GetBatteryLevel();
            var signal = SycaBackgroundTask.GeoPhoneContextProvider.GetSignalStrength();
            var dataConnectionEnable = SycaBackgroundTask.GeoPhoneContextProvider.IsDataConnectionEnable();
            var lbsDisable = SycaBackgroundTask.GeoPhoneContextProvider.IsLocationServicesDISABLE();
        }

        private static void ShowToast()
        {
            ToastTemplateType toastType = ToastTemplateType.ToastText02;

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastType);
            XmlNodeList toastTextElement = toastXml.GetElementsByTagName("text");
            toastTextElement[0].AppendChild(toastXml.CreateTextNode("Hello C# Corner"));
            toastTextElement[1].AppendChild(toastXml.CreateTextNode("I am poping you from a Winmdows Phone App"));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }
    }
}

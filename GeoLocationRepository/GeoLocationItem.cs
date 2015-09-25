using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace GeoLocationRepository
{
    [SQLite.Table("GeoLocationItem")]
    public class GeoLocationItem
    {

        #region ctor 

        public GeoLocationItem() { }

        public GeoLocationItem( int id,
                                double? latitude, double? logitude, DateTime eventDate, int signalStrength, 
                                int batteryLevel, bool locationServicesEnanble, bool dataConnectionEnable,
                                bool sentServer , DateTime ? sentDate
                               )
        {
            this.Id = id;
            this.Latitude = Latitude;
            this.Longitude = logitude;
            this.EventDate = eventDate;
            this.SignalStrength = signalStrength;
            this.BatteryLevel = batteryLevel;
            this.LocationServicesEnable = locationServicesEnanble;
            this.DataConnectionEnable = dataConnectionEnable;
            this.SentServer = sentServer;
            this.SentDate = sentDate;
        }

        #endregion

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public double? Latitude { get;  set; }
        public double? Longitude { get;  set; }
        public DateTime EventDate { get;  set; }
        public int SignalStrength { get;  set; }
        public int BatteryLevel { get;  set; }
        public bool LocationServicesEnable { get;  set; }
        public bool DataConnectionEnable { get;  set; }
        public bool SentServer { get;  set; }
        public DateTime? SentDate { get;  set; }
    }

  


}

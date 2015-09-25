using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GeoLocationRepository
{
    public class DatabaseHelper
    {
        #region properties


        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "trackerDB.sqlite"));//DataBase Name 

        SQLiteConnection dbConn;

        #endregion

        #region ctor

        public DatabaseHelper() 
        {

        }

        #endregion

        #region public methods

        public async Task<bool> OnCreate()
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<GeoLocationItem>();
                    }
                    
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public GeoLocationItem GetGeoLocationItem(int id)
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                var existingconact = dbConn.Query<GeoLocationItem>("SELECT * FROM GeoLocationItem where Id =" + id).FirstOrDefault();
                return existingconact;
            }
        }

        public ObservableCollection<GeoLocationItem> GetAll()
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                List<GeoLocationItem> myCollection = dbConn.Table<GeoLocationItem>().ToList<GeoLocationItem>();
                ObservableCollection<GeoLocationItem> ContactsList = new ObservableCollection<GeoLocationItem>(myCollection);
                return ContactsList;
            }   
            
        }

        public async Task<GeoLocationItem> InsertAsync(GeoLocationItem item) 
        {

            var asyncConnect = new SQLiteAsyncConnection(DB_PATH);

            int insertedId  = await asyncConnect.InsertAsync(item);

            item.Id = insertedId;

            return item;
            
            /*using (var dbConn = new SQLite.SQLiteConnection(DB_PATH))
            {
                int insertedId = dbConn.Insert(item);
                item.Id = insertedId;
                return item;
            }*/
        }

        public GeoLocationItem Insert(GeoLocationItem item)
        {
            using (var dbConn = new SQLite.SQLiteConnection(DB_PATH))
            {
                int insertedId = dbConn.Insert(item);
                item.Id = insertedId;
                return item;
            }
        }

        public int Delete(int id) 
        {
            var match = this.GetGeoLocationItem(id);

            if (match == null)
                throw new Exception("GeoLocation not found");

            int deletedRows = -1;
            using (dbConn = new SQLiteConnection(DB_PATH))
            {
                 deletedRows= dbConn.Delete(match);
            }
            return deletedRows;
        }

        public GeoLocationItem Update(GeoLocationItem item) 
        {
            if( GetGeoLocationItem(item.Id) == null)
                throw new Exception("GeoLocation not found");

            using (dbConn = new SQLiteConnection(DB_PATH))
            {
                dbConn.Update(item);
            }
            return item;
        }

        #endregion

        #region private methods 
        
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #endregion

    }
}

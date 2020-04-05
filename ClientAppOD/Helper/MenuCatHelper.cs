using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClientAppOD.CustomModels;
using OD.Data;
using SQLite;
using Xamarin.Forms;

namespace ClientAppOD.Helper
{
    public class MenuCatHelper
    {
        private static SQLiteAsyncConnection _connection;
        private static ObservableCollection<MenuCategory> _menuCategories { get; set; }
        private static SelectedStore store;
        public static string MenuString
        {
            get;set;
        }
        public async  Task<string> GetMenu(int Id)
        {
            string url = StaticFields.ServerURL+"/api/store/get?id="+Id;
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = await Task.Run(async () => await req.GetResponseAsync());
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return await Task.Run(async () => await sr.ReadToEndAsync());
        }
        public static ObservableCollection<MenuCategory> MenuCategories
        {
            get
            {
                return _menuCategories;
            }
            set
            {
                _menuCategories = value;
            }
        }
        private static async Task<string> GetMenuCat()
        {
            await _connection.CreateTableAsync<SelectedStore>();
            store = await _connection.Table<SelectedStore>().FirstOrDefaultAsync();
            
            return "";
        }
        
    }
}

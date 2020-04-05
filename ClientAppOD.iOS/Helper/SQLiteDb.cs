using System;
using System.IO;
using ClientAppOD.Helper;
using ClientAppOD.iOS.Helper;
using SQLite;
using Xamarin.Forms;

[assembly:Dependency(typeof(SQLiteDb))]
namespace ClientAppOD.iOS.Helper
{
    public class SQLiteDb: ISQLiteLdb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var DocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            var path = Path.Combine(DocumentPath, "ODMain1.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}

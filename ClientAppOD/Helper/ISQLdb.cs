using System;
using SQLite;

namespace ClientAppOD.Helper
{
    public interface ISQLiteLdb
    {
        SQLiteAsyncConnection GetConnection();
    }
}

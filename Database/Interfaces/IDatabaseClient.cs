using System;
using MySql.Data.MySqlClient;

namespace StarBlue.Database.Interfaces
{
    public interface IDatabaseClient : IDisposable
    {
        void connect();
        void disconnect();
        IQueryAdapter getQueryreactor();
        MySqlCommand createNewCommand();
        void reportDone();
    }
}
using MySql.Data.MySqlClient;
using System;

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
using Database_Manager.Session_Details.Interfaces;
using System;

namespace Database_Manager.Database.Session_Details.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        void doRollBack();
        void doCommit();

        long InsertQuery();
        void RunQuery();
    }
}

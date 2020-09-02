using System;

namespace StarBlue.Database.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        long InsertQuery();
    }
}
using Database_Manager.Database.Session_Details.Interfaces;

namespace Database_Manager.Database.Session_Details
{
    class NormalQueryReactor : QueryAdapter, IQueryAdapter
    {
        #region Constructor
        /// <summary>
        /// Creates a new Database session
        /// </summary>
        /// <param name="client">The client which holds the connection</param>
        internal NormalQueryReactor(DatabaseClient client) : base(client)
        {
            command = client.getNewCommand();
        }
        #endregion

        #region Auto commit information
        /// <summary>
        /// Gets the auto commit information about this item
        /// </summary>
        /// <returns></returns>
        internal bool getAutoCommit()
        {
            return true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the current item
        /// </summary>
        public void Dispose()
        {
            command.Dispose();
            client.reportDone();
        }

        #endregion

        #region Query kinds

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">Every time, this is not supported on a normal query reactor</exception>
        public void doRollBack()
        {
            new Database_Exceptions.TransactionException("Can't use rollback on a non-transactional Query reactor");
        }

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">Every time, this is not supported on a normal query reactor</exception>
        public void doCommit()
        {
            new Database_Exceptions.TransactionException("Can't use rollback on a non-transactional Query reactor");
        }

        #endregion
    }
}

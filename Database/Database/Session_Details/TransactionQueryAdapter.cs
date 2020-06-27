using Database_Manager.Database.Session_Details.Interfaces;
using MySql.Data.MySqlClient;


namespace Database_Manager.Database.Session_Details
{
    class TransactionQueryReactor : QueryAdapter, IQueryAdapter
    {
        #region Declares
        /// <summary>
        /// Indicates if all queries where correct
        /// </summary>
        private bool finishedTransaction;

        /// <summary>
        /// The transaction of the current item
        /// </summary>
        private MySqlTransaction transaction;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Database session
        /// </summary>
        /// <param name="client">The client which holds the connection</param>
        internal TransactionQueryReactor(DatabaseClient client) : base(client)
        {
            initTransaction();
        }
        #endregion

        #region Auto commit information
        /// <summary>
        /// Gets the auto commit information about this item
        /// </summary>
        /// <returns></returns>
        internal bool getAutoCommit()
        {
            return false;
        }

        /// <summary>
        /// Sets the auto commit of the current item
        /// </summary>
        /// <param name="commit">The new autocommit value</param>
        private void initTransaction()
        {
            command = client.getNewCommand();
            transaction = client.getTransaction();
            command.Transaction = transaction;
            command.Connection = transaction.Connection;
            finishedTransaction = false;
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the current item
        /// </summary>
        public void Dispose()
        {
            if (!finishedTransaction)
            {
                throw new Database_Exceptions.TransactionException("The transaction needs to be completed by commit() or rollback() before you can dispose this item.");
            }
            command.Dispose();
            client.reportDone();
        }

        #endregion

        #region Query kinds

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">When an rollback action fails</exception>
        public void doRollBack()
        {
            try
            {
                transaction.Rollback();
                finishedTransaction = true;
            }
            catch (MySqlException ex)
            {
                throw new Database_Exceptions.TransactionException(ex.Message);
            }
        }

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">When an commit action fails</exception>
        public void doCommit()
        {
            try
            {
                transaction.Commit();
                finishedTransaction = true;
            }
            catch (MySqlException ex)
            {
                throw new Database_Exceptions.TransactionException(ex.Message);
            }
        }

        #endregion
    }
}

namespace Database_Manager.Session_Details.Interfaces
{
    public interface IRegularQueryAdapter
    {
        /// <summary>
        /// Sets a new query
        /// Query parameters can be added by using the @-sign (see example)
        /// By setting the new query you clear the old parameter values
        /// </summary>
        /// <param name="query">The new query</param>
        /// <example>
        /// SELECT field FROM table WHERE id = @id 
        /// Using addParameter(id, 10) will result in 
        /// SELECT field FROM table WHERE id = 10
        /// using addParameter will result in safe queries, so no escaping is needed
        /// </example>
        void SetQuery(string query);

        /// <summary>
        /// Adds a parameter to the current command 
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="val">The value of the parameter</param>
        /// <example>
        /// If the query is "SELECT field FROM table WHERE id = @id" and you would use
        /// addParameter(id, 10);
        /// The query will look like
        /// SELECT field FROM table WHERE id = 10
        /// </example>
        void AddParameter(string name, object query);

        /// <summary>
        /// Gets a datatable using the query
        /// </summary>
        /// <returns>The datatable generated with the query</returns>
        /// <exception cref="QueryException">
        /// Throws an exception if there was an error with the query
        /// Or there was no query given
        /// </exception>
        System.Data.DataTable GetTable();

        /// <summary>
        /// Gets a row from the database
        /// </summary>
        /// <returns>The datarow</returns>
        System.Data.DataRow GetRow();

        /// <summary>
        /// Gets an integer from the database
        /// </summary>
        /// <returns>an integer or 0 on no data found</returns>
        int GetInteger();

        /// <summary>
        /// gets a string from the database
        /// </summary>
        /// <returns></returns>
        string GetString();

        /// <summary>
        /// Runs a fast query
        /// </summary>
        /// <param name="query">The query</param>
        void RunFastQuery(string query);

        /// <summary>
        /// Finds a result in the database
        /// </summary>
        /// <returns>Indication if the item is found or not</returns>
        bool FindsResult();
    }
}

using Database_Manager.Database.Database_Exceptions;
using Database_Manager.Database.Session_Details.Interfaces;
using Database_Manager.Managers.Database;
using MySql.Data.MySqlClient;
using StarBlue.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Database_Manager.Database
{
    public class DatabaseManager
    {
        /// <summary>
        /// The server details of the database
        /// </summary>
        private DatabaseServer server;

        /// <summary>
        /// Indication of how many connections the server may have with the mysql host
        /// </summary>
        private uint maxPoolSize;

        /// <summary>
        /// Indication of the startamount of clients which will be connected to the database server at initialization
        /// </summary>
        private int beginClientAmount;

        /// <summary>
        /// The connection string which is used to connect to the database server
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Indicates if this item is connected
        /// </summary>
        private bool isConnected = false;

        /// <summary>
        /// dbClients currently unused
        /// </summary>
        private Queue dbClientCollection;
        /// <summary>
        /// Contains the stack of "broken/expired" conenctions
        /// </summary>
        private Queue refreshStack;

        private int totalCurrentConnections = 0;

        private Thread checkThread;

        /// <summary>
        /// Creates a new database manager with a max-poolsize and begin client amount
        /// </summary>
        /// <param name="maxPoolSize">An unsigned int with the indication of the max amount of connections to the database server</param>
        /// <param name="clientAmount">An unsigned int with the indication of the clients which will be connected for the server</param>
        public DatabaseManager(uint maxPoolSize, int clientAmount)
        {
            Logging.WriteLine("Setting up Database manager with a maximum of [" + maxPoolSize + "] and start client amount of [" + clientAmount + "]");
            if (maxPoolSize < clientAmount)
            {
                throw new Database_Exceptions.DatabaseException("The poolsize can not be larger than the client amount!");
            }

            beginClientAmount = clientAmount;
            this.maxPoolSize = maxPoolSize;
        }

        /// <summary>
        /// Sets the server detials
        /// </summary>
        /// <param name="host">The network host of the database server, eg 'localhost' or '127.0.0.1'.</param>
        /// <param name="port">The network port of the database server as integer.</param>
        /// <param name="username">The username to use when connecting to the database.</param>
        /// <param name="password">The password to use in combination with the username when connecting to the database.</param>
        /// <returns>A boolean indicating if the data was filled in propperly</returns>
        public bool setServerDetails(string host, uint port, string username, string password, string databaseName)
        {
            try
            {
                server = new DatabaseServer(host, port, username, password, databaseName);
                Logging.WriteLine("Database connector succesfully set the connection details.", ConsoleColor.DarkGreen);
                return true;
            }
            catch (DatabaseException)
            {
                isConnected = false;

                return false;
            }
        }

        /// <summary>
        /// Initializes the connections
        /// </summary>
        /// <returns>boolean indicating if the initialization process was succesfull</returns>
        public void init()
        {
            if (isConnected)
            {
                return;
            }

            try
            {
                Logging.WriteLine("Creating new database connections.. stand by..", ConsoleColor.DarkGray);

                createNewConnectionString();

                dbClientCollection = new Queue();
                refreshStack = new Queue();
                DatabaseClient dbClient;
                if (beginClientAmount != 0)
                {
                    for (int i = 0; i <= beginClientAmount; i++)
                    {
                        Logging.WriteLine("Opening database connection [" + (i).ToString() + "] out of [" + beginClientAmount + "]", ConsoleColor.DarkGray);
                        addConnection();

                    }
                }
                else
                {
                    dbClient = new DatabaseClient(this, -1);
                    dbClient.connect();
                    dbClient.disconnect();
                }
                //Out.writePlain(connectionString, Out.logFlags.lowLogLevel);

            }
            catch (MySqlException ex)
            {
                isConnected = false;
                throw new Exception("Could not connect the clients to the database: " + ex.Message);
            }

            isConnected = true;
            checkThread = new Thread(healthChecker);
            checkThread.Start();
            Logging.WriteLine("Created new connections: [" + beginClientAmount + "]");


        }

        /// <summary>
        /// Gets the connections string
        /// </summary>
        /// <returns>The connection string used to connect to a database server</returns>
        internal string getConnectionString()
        {
            return connectionString;
        }

        /// <summary>
        /// Returns a prepared database connection
        /// </summary>
        /// <param name="autoCommit"></param>
        /// <returns></returns>
        private DatabaseClient getClient(bool autoCommit)
        {
            try
            {
                lock (dbClientCollection.SyncRoot)
                {
                    DateTime now = DateTime.Now;
                    DatabaseClient toReturn;
                    while (dbClientCollection.Count > 0)
                    {
                        toReturn = dbClientCollection.Dequeue() as DatabaseClient;
                        if (toReturn.getConnectionState() == ConnectionState.Open && (now - toReturn.getLastAction()).TotalHours < 3)
                        {
                            toReturn.prepare(autoCommit);
                            return toReturn;
                        }
                        else
                        {
                            lock (refreshStack.SyncRoot)//database client is borked
                            {
                                refreshStack.Enqueue(toReturn);
                            }
                        }
                    }

                    if (totalCurrentConnections < maxPoolSize)
                    {
                        addConnection();
                        return getClient(autoCommit);
                    }
                    else
                    {
                        maxPoolSize = (uint)(maxPoolSize * 1.5);
                        Logging.WriteLine("Out of clients, POOL SIZE TOO SMALL!! INCREASE IT TO [" + maxPoolSize + "] for propper operation..", ConsoleColor.DarkGray);
                        createNewConnectionString();
                        return getClient(autoCommit);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLine("problem? " + ex.Message);
                return null;
            }

        }

        private void addConnection()
        {
            DatabaseClient dbClient = new DatabaseClient(this, totalCurrentConnections);
            totalCurrentConnections++;
            try
            {
                dbClient.connect();
            }
            catch (Exception ex)
            {
                Logging.WriteLine("Error while connecting to database: " + ex.Message);
            }

            dbClientCollection.Enqueue(dbClient);
            Logging.WriteLine("Adding new client to the database manager.", ConsoleColor.DarkGray);
        }

        private void setConnectionString(MySqlConnectionStringBuilder connectionString)
        {
            this.connectionString = connectionString.ToString();
        }

        private void createNewConnectionString()
        {
            MySqlConnectionStringBuilder mysqlSb = new MySqlConnectionStringBuilder
            {
                Server = server.getHost(),
                Port = server.getPort(),
                UserID = server.getUsername(),
                Password = server.getPassword(),

                Database = server.getDatabaseName(),
                MinimumPoolSize = (maxPoolSize / 2),
                MaximumPoolSize = maxPoolSize,
                AllowZeroDateTime = true,
                ConvertZeroDateTime = true,
                Pooling = true,
                SslMode = MySqlSslMode.None,
                Logging = true,
                DefaultCommandTimeout = 30,
                ConnectionTimeout = 10
            };

            setConnectionString(mysqlSb);

        }

        /// <summary>
        /// Gets a query reactor with a boolean indicating its a transaction or not
        /// </summary>
        /// <param name="autoCommit">Indicates if this is going to be a transaction or not</param>
        /// <returns>An interface with the transaction/normal query reactor</returns>
        public IQueryAdapter GetQueryReactor(bool transaction)
        {
            return getClient(transaction).getQueryReactor();
        }
        /// <summary>
        /// Gets a query reactor with a boolean indicating its a transaction or not
        /// </summary>
        /// <param name="autoCommit">Indicates if this is going to be a transaction or not</param>
        /// <returns>An interface with the transaction/normal query reactor</returns>
        public IQueryAdapter GetQueryReactor()
        {
            return GetQueryReactor(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void destroy()
        {
            lock (dbClientCollection.SyncRoot)
            {
                isConnected = false;
                if (dbClientCollection != null)
                {
                    foreach (DatabaseClient client in dbClientCollection)
                    {
                        if (!client.isAvailable())
                        {
                            client.Dispose();
                        }
                        client.disconnect();
                    }
                    dbClientCollection.Clear();
                }

            }
        }

        /// <summary>
        /// Boolean indicating if this item is connected to the database
        /// </summary>
        /// <returns></returns>
        public bool isConnectedToDatabase()
        {
            return isConnected;
        }

        private void healthChecker()
        {
            while (isConnected)
            {
                if (refreshStack.Count > 0)
                {
                    lock (refreshStack.SyncRoot)
                    {
                        if (refreshStack.Count > 0)
                        {
                            DatabaseClient toReCheck;
                            List<DatabaseClient> failedClients = new List<DatabaseClient>(refreshStack.Count);
                            while (refreshStack.Count > 0)
                            {
                                toReCheck = refreshStack.Dequeue() as DatabaseClient;
                                toReCheck.disconnect();
                                toReCheck.Dispose();
                                toReCheck = new DatabaseClient(this, toReCheck.getID());
                                try
                                {
                                    toReCheck.connect();

                                    reportDone(toReCheck);
                                }
                                catch (Exception)
                                {
                                    failedClients.Add(toReCheck);
                                }
                            }
                            foreach (DatabaseClient client in failedClients)
                            {
                                refreshStack.Enqueue(client);
                            }
                        }
                    }
                }
                Thread.Sleep(100);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getOpenConnectionCount()
        {
            return totalCurrentConnections;
        }

        internal void reportDone(DatabaseClient databaseClient)
        {
            lock (dbClientCollection.SyncRoot)
            {
                dbClientCollection.Enqueue(databaseClient);
            }
        }

        public void dumpStatus(ref StringBuilder builder)
        {
            //if(toReturn.getConnectionState() == ConnectionState.Open && (now - toReturn.getLastAction()).TotalHours < 3)

            Console.WriteLine("=== DATABASE CONNECTOR STATUS ===");
            builder.AppendLine("=== DATABASE CONNECTOR STATUS ===");
            Console.WriteLine("Obtaining lock on dbClientConnection.SyncRoot");
            builder.AppendLine("Obtaining lock on dbClientConnection.SyncRoot");

            int readyConnection = 0;

            lock (dbClientCollection.SyncRoot)
            {
                Console.WriteLine("Lock obtained on dbClientConnection.SyncRoot");
                builder.AppendLine("Lock obtained on dbClientConnection.SyncRoot");
                DateTime now = DateTime.Now;
                foreach (DatabaseClient dbClient in dbClientCollection)
                {
                    bool wouldReturn = false;

                    double idleHours = (now - dbClient.getLastAction()).TotalHours;
                    if (dbClient.getConnectionState() == ConnectionState.Open && idleHours < 3)
                    {
                        wouldReturn = true;
                    }

                    Console.WriteLine("Would return: " + wouldReturn.ToString());
                    builder.AppendLine("Would return: " + wouldReturn.ToString());

                    Console.WriteLine("Connection state: " + dbClient.getConnectionState().ToString());
                    builder.AppendLine("Connection state: " + dbClient.getConnectionState().ToString());

                    Console.WriteLine("Idle time: " + wouldReturn);
                    builder.AppendLine("Idle time: " + wouldReturn);
                    readyConnection++;
                }
            }

            Console.WriteLine("Ready connections: " + readyConnection);
            builder.AppendLine("Ready connections: " + readyConnection);

            Console.WriteLine("Obtaining lock on refreshStack.SyncRoot");
            builder.AppendLine("Obtaining lock on refreshStack.SyncRoot");

            int count = 0;

            lock (refreshStack.SyncRoot)
            {
                Console.WriteLine("Obtained lock on refreshStack.SyncRoot");
                builder.AppendLine("Obtained lock on refreshStack.SyncRoot");

                DateTime now = DateTime.Now;
                foreach (DatabaseClient dbClient in refreshStack)
                {
                    bool wouldReturn = false;

                    double idleHours = (now - dbClient.getLastAction()).TotalHours;
                    if (dbClient.getConnectionState() == ConnectionState.Open && idleHours < 3)
                    {
                        wouldReturn = true;
                    }

                    Console.WriteLine("Would return: " + wouldReturn.ToString());
                    builder.AppendLine("Would return: " + wouldReturn.ToString());

                    Console.WriteLine("Connection state: " + dbClient.getConnectionState().ToString());
                    builder.AppendLine("Connection state: " + dbClient.getConnectionState().ToString());

                    Console.WriteLine("Idle time: " + wouldReturn);
                    builder.AppendLine("Idle time: " + wouldReturn);
                    count++;
                }
            }
            Console.WriteLine("Waiting refresh count: " + count);
            builder.AppendLine("Waiting refresh count: " + count);

            Console.WriteLine("");
            builder.AppendLine("");
        }
    }
}

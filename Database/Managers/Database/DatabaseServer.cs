namespace Database_Manager.Managers.Database
{
    using Database_Manager.Database.Database_Exceptions;

    public class DatabaseServer
    {
        private readonly string databaseName;
        private readonly string host;
        private readonly string password;
        private readonly uint port;
        private readonly string user;

        public DatabaseServer(string host, uint port, string username, string password, string databaseName)
        {
            if ((host == null) || (host.Length == 0))
            {
                throw new DatabaseException("No host was given");
            }
            if ((username == null) || (username.Length == 0))
            {
                throw new DatabaseException("No username was given");
            }
            if ((databaseName == null) || (databaseName.Length == 0))
            {
                throw new DatabaseException("No database name was given");
            }
            this.host = host;
            this.port = port;
            this.databaseName = databaseName;
            user = username;
            this.password = (password != null) ? password : "";
        }

        public string getDatabaseName()
        {
            return databaseName;
        }

        public string getHost()
        {
            return host;
        }

        public string getPassword()
        {
            return password;
        }

        public uint getPort()
        {
            return port;
        }

        public string getUsername()
        {
            return user;
        }

        public override string ToString()
        {
            return (user + "@" + host);
        }
    }
}


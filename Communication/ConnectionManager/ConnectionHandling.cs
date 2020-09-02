using StarBlue.Core;
using StarBlueServer.Communication;
using System;

namespace StarBlue.Communication.ConnectionManager
{
    public class ConnectionHandling
    {
        public GameSocketManager manager;

        public ConnectionHandling(int port, int connectionsPerIP)
        {
            this.manager = new GameSocketManager();
            this.manager.init(port, connectionsPerIP, new InitialPacketParser());

            this.manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.connectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            StarBlueServer.GetGame().GetClientManager().CreateAndStartClient(connection.getConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);

            information.connectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        public void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                StarBlueServer.GetGame().GetClientManager().DisposeConnection(Connection.getConnectionID());

                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }


        public void Destroy()
        {
            this.manager.Destroy();
        }
    }
}

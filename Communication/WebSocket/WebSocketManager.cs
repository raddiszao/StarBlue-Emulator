using StarBlue.Communication.ConnectionManager;
using StarBlue.Core;
using System;

namespace StarBlue.Communication.WebSocket
{
    public class WebSocketManager
    {
        public GameSocketManager manager;

        public WebSocketManager(int port, int connectionsPerIP)
        {
            this.manager = new GameSocketManager();
            this.manager.init(port, connectionsPerIP, new InitialPacketParser());
            this.manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.connectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            StarBlueServer.GetGame().GetWebClientManager().CreateAndStartClient(connection.getConnectionID(), connection);
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
                StarBlueServer.GetGame().GetWebClientManager().DisposeConnection(Connection.getConnectionID());

                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }


        public void destroy()
        {
            this.manager.Destroy();
        }
    }
}
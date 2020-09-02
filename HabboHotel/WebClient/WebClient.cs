using StarBlue.Communication;
using StarBlue.Communication.ConnectionManager;
using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.Communication.WebSocket;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlueServer.Communication;
using System;
using System.Data;

namespace StarBlue.HabboHotel.WebClient
{
    public class WebClient
    {
        private ConnectionInformation _connection;
        private WebPacketParser _packetParser;

        public int UserId;

        public int ConnectionID;

        public WebClient(int id, ConnectionInformation connection)
        {
            this.ConnectionID = id;
            this._connection = connection;
            this._packetParser = new WebPacketParser(this);
        }

        public void TryAuthenticate(string AuthTicket)
        {
            if (string.IsNullOrEmpty(AuthTicket))
                return;

            using (IQueryAdapter queryreactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id FROM users WHERE auth_ticket = @sso");
                queryreactor.AddParameter("sso", AuthTicket);

                DataRow dUserInfo = queryreactor.GetRow();
                if (dUserInfo == null)
                    return;

                this.UserId = Convert.ToInt32(dUserInfo["id"]);
            }

            StarBlueServer.GetGame().GetWebClientManager().LogClonesOut(UserId);
            StarBlueServer.GetGame().GetWebClientManager().RegisterClient(this, UserId);
            this.SendPacket(new AuthOkComposer());
        }

        private void SwitchParserRequest()
        {
            this._packetParser.onNewPacket += new WebPacketParser.HandlePacket(this.parser_onNewPacket);

            byte[] packet = (this._connection.parser as InitialPacketParser).currentData;
            this._connection.parser.Dispose();
            this._connection.parser = (IDataParser)this._packetParser;
            this._connection.parser.handlePacketData(packet);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                StarBlueServer.GetGame().GetPacketManager().TryExecuteWebPacket(this, Message);
            }
            catch (Exception ex)
            {
                Logging.LogPacketException(Message.ToString(), (ex).ToString());
            }
        }

        public ConnectionInformation GetConnection()
        {
            return this._connection;
        }

        public void StartConnection()
        {
            if (this._connection == null)
                return;

            (this._connection.parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this._connection.startPacketProcessing();
        }

        public void Dispose()
        {
            if (this._connection != null)
                this._connection.Dispose();
        }

        public void SendPacket(IServerPacket Message)
        {
            if (Message == null || this.GetConnection() == null) return;

            this.GetConnection().SendData(EncodeDecode.EncodeMessage(Message.GetBytes()));
        }
    }
}

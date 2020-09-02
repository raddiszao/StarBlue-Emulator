using StarBlue.Communication.ConnectionManager;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Messages
{
    public class QueuedServerMessage
    {
        private List<byte> packet;
        private ConnectionInformation userConnection;
        private Room Room;

        public QueuedServerMessage(ConnectionInformation connection)
        {
            this.userConnection = connection;
            this.packet = new List<byte>();
        }

        public QueuedServerMessage(Room Room)
        {
            this.Room = Room;
            this.packet = new List<byte>();
        }

        internal void Dispose()
        {
            packet.Clear();
            userConnection = null;
            Room = null;
        }

        internal void appendResponse(ServerPacket message)
        {
            packet.AddRange(message.GetBytes());
        }

        internal void sendResponse()
        {
            if (this.Room == null)
                this.userConnection.SendData(packet.ToArray());
            else
                this.Room.BroadcastPacket(packet.ToArray());
            Dispose();
        }
    }
}

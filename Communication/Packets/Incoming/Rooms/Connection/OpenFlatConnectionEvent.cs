using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            Session.GetHabbo().PrepareRoom(RoomId, Password);
        }
    }
}
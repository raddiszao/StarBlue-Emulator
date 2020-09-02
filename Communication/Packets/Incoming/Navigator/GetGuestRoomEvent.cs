using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();

            RoomData roomData = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
            {
                return;
            }

            bool isLoading = Packet.PopInt() == 1;
            bool checkEntry = Packet.PopInt() == 1;

            Session.SendMessage(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
        }
    }
}

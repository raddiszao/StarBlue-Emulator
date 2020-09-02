
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();
            if (roomID == 0)
            {
                return;
            }

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (Data == null)
            {
                return;
            }

            Session.GetHabbo().HomeRoom = roomID;
            Session.SendMessage(new NavigatorSettingsComposer(roomID));
        }
    }
}

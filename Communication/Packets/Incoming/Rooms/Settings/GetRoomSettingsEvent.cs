
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    internal class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            Session.SendMessage(new RoomSettingsDataComposer(Room));
        }
    }
}

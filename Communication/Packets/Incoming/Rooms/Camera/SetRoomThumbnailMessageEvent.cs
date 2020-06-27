using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Camera
{
    public class SetRoomThumbnailMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            Session.SendMessage(new SendRoomThumbnailAlertMessageComposer());
        }
    }

}


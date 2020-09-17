using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Camera
{
    public class ParticipatePictureCameraCompetitionMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            //Session.SendMessage(new CameraFinishParticipateCompetitionMessageComposer(true, ""));
        }
    }
}
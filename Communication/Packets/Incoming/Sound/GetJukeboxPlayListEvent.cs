using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class GetJukeboxPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
            {
                Session.SendMessage(new SetJukeboxPlayListComposer(Session.GetHabbo().CurrentRoom));
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_MusicPlayer", 1);
            }
        }
    }
}

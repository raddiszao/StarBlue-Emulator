using StarBlue.Communication.Packets.Outgoing.Inventory.Achievements;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Achievements
{
    internal class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new AchievementsComposer(Session.GetHabbo(), StarBlueServer.GetGame().GetAchievementManager()._achievements.Values.ToList()));
        }
    }
}

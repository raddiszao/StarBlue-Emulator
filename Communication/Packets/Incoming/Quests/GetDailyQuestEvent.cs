using StarBlue.Communication.Packets.Outgoing.LandingView;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.Communication.Packets.Incoming.Quests
{
    internal class GetDailyQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int UsersOnline = StarBlueServer.GetGame().GetClientManager().Count;

            int Goal = Convert.ToInt32(StarBlueServer.GetSettingsManager().TryGetValue("usersconcurrent_goal"));
            Session.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline, UsersOnline >= Goal ? 2 : 1, Goal));
        }
    }
}

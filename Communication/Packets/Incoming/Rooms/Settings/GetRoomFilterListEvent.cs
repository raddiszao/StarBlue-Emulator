
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    internal class GetRoomFilterListEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Instance = Session.GetHabbo().CurrentRoom;
            if (Instance == null)
            {
                return;
            }

            if (!Instance.CheckRights(Session))
            {
                return;
            }

            Session.SendMessage(new GetRoomFilterListComposer(Instance.WordFilterList));
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModRoomFilterSeen", 1);
        }
    }
}

using StarBlue.HabboHotel.Navigator;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    class SaveEnforcedCategorySettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            int CategoryId = Packet.PopInt();
            int TradeSettings = Packet.PopInt();

            if (TradeSettings < 0 || TradeSettings > 2)
            {
                TradeSettings = 0;
            }

            if (!StarBlueServer.GetGame().GetNavigator().TryGetSearchResultList(CategoryId, out SearchResultList SearchResultList))
            {
                CategoryId = 36;
            }

            if (SearchResultList.CategoryType != NavigatorCategoryType.CATEGORY || SearchResultList.RequiredRank > Session.GetHabbo().Rank)
            {
                CategoryId = 36;
            }
        }
    }
}

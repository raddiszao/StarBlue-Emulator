
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    internal class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            int ItemId = Packet.PopInt();
            int GroupId = Packet.PopInt();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (Item.Data.InteractionType != InteractionType.GUILD_GATE)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.SendMessage(new GroupFurniSettingsComposer(Group, ItemId, Session.GetHabbo().Id));
            Session.SendMessage(new GroupInfoComposer(Group, Session, false));
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.Wired
{
    internal class SaveWiredConfigEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int ItemId = Packet.PopInt();

            Session.SendMessage(new HideWiredConfigComposer());

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, false, true) && !Room.CheckRights(Session, true))
            {
                return;
            }

            Item SelectedItem = Room.GetRoomItemHandler().GetItem(ItemId);
            if (SelectedItem == null)
            {
                return;
            }

            if (!Session.GetHabbo().CurrentRoom.GetWired().TryGet(ItemId, out IWiredItem Box))
            {
                return;
            }

            if (Box.Type == WiredBoxType.EffectGiveUserBadge && !Session.GetHabbo().GetPermissions().HasRight("room_item_wired_rewards"))
            {
                Session.SendNotification("Você não tem permissões suficientes para usar este Wired.");
                return;
            }

            Box.HandleSave(Packet);
            Room.GetWired().SaveBox(Box);
        }
    }
}

using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    internal class ThrowDiceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null && Item.Data.InteractionType != InteractionType.DICE)
            {
                return;
            }

            if (!Item.ExtraData.Equals("-1"))
            {
                Item.Interactor.OnTrigger(Session, Item, 0, Room.CheckRights(Session, false, true));
            }
        }
    }
}
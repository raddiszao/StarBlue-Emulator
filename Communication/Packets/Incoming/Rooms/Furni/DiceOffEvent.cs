using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    internal class DiceOffEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null)
            {
                return;
            }

            bool hasRights = false;
            if (Room.CheckRights(Session))
            {
                hasRights = true;
            }

            Item.Interactor.OnTrigger(Session, Item, -1, hasRights);
        }
    }
}

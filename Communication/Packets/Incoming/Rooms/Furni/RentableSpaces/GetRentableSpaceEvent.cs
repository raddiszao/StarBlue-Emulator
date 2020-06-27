
using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.RentableSpaces;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room room))
            {
                return;
            }

            Item item = room.GetRoomItemHandler().GetItem(ItemId);

            if (item == null)
            {
                return;
            }

            if (item.GetBaseItem() == null)
            {
                return;
            }

            if (item.GetBaseItem().InteractionType != InteractionType.RENTABLE_SPACE)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(ItemId, out RentableSpaceItem _rentableSpace))
            {
                _rentableSpace = StarBlueServer.GetGame().GetRentableSpaceManager().CreateAndAddItem(ItemId, Session);
            }

            if (_rentableSpace.Rented)
            {
                Session.SendMessage(new RentableSpaceComposer(_rentableSpace.OwnerId, _rentableSpace.GetExpireSeconds()));
            }
            else
            {
                int errorCode = StarBlueServer.GetGame().GetRentableSpaceManager().GetButtonErrorCode(Session, _rentableSpace);
                Session.SendMessage(new RentableSpaceComposer(false, errorCode, -1, 0, _rentableSpace.Price));
            }
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.RentableSpaces;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    internal class CancelRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {

            int itemId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room room))
            {
                return;
            }

            if (room == null || room.GetRoomItemHandler() == null)
            {
                return;
            }

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out RentableSpaceItem _rentableSpace))
            {
                return;
            }

            int errorCode = StarBlueServer.GetGame().GetRentableSpaceManager().GetCancelErrorCode(Session, _rentableSpace);

            if (errorCode > 0)
            {
                Session.SendMessage(new RentableSpaceComposer(_rentableSpace.IsRented(), errorCode, _rentableSpace.OwnerId, _rentableSpace.GetExpireSeconds(), _rentableSpace.Price));
                return;
            }


            if (!StarBlueServer.GetGame().GetRentableSpaceManager().ConfirmCancel(Session, _rentableSpace))
            {
                Session.SendNotification("global.error");
                return;
            }

            Session.SendMessage(new RentableSpaceComposer(false, 0, 0, 0, _rentableSpace.Price));
        }
    }
}
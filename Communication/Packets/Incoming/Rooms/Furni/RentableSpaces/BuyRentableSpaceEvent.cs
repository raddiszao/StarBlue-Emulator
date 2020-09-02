using StarBlue.HabboHotel.Items.RentableSpaces;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    internal class BuyRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
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

            if (StarBlueServer.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out RentableSpaceItem rsi))
            {
                StarBlueServer.GetGame().GetRentableSpaceManager().ConfirmBuy(Session, rsi, 3600);
            }


        }
    }
}
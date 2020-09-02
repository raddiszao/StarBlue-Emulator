
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class MoveWallItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session) || Room.ForSale)
            {
                Session.SendNotification("Não foi possível mover o item, tente novamente.");
                return;
            }

            int itemID = Packet.PopInt();
            string wallPositionData = Packet.PopString();

            Item Item = Room.GetRoomItemHandler().GetItem(itemID);

            if (Item == null)
            {
                return;
            }

            string WallPos = Room.GetRoomItemHandler().WallPositionCheck(":" + wallPositionData.Split(':')[1]);
            Item.wallCoord = WallPos;
            Room.GetRoomItemHandler().UpdateItem(Item);
            Room.SendMessage(new ItemUpdateComposer(Item, Room.RoomData.OwnerId));
        }
    }
}

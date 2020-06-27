
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    class UpdateMagicTileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, false, true) && !Session.GetHabbo().GetPermissions().HasRight("room_item_use_any_stack_tile"))
            {
                return;
            }

            int ItemId = Packet.PopInt();
            int DecimalHeight = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            Item.GetZ = DecimalHeight / 100.0;

            if (Item.GetZ < Room.GetGameMap().Model.SqFloorHeight[Item.GetX, Item.GetY])
            {
                Item.GetZ = Room.GetGameMap().Model.SqFloorHeight[Item.GetX, Item.GetY];
            }

            Item.SetState(Item.GetX, Item.GetY, Item.GetZ, Item.GetAffectedTiles);
            foreach (ThreeDCoord value in Item.GetAffectedTiles.Values)
            {
                Room.GetGameMap().ItemHeightMap[value.X, value.Y] = Item.GetZ;
            }

            Room.SendMessage(new ObjectUpdateComposer(Item, Session.GetHabbo().Id));
            Room.SendMessage(new UpdateMagicTileComposer(ItemId, DecimalHeight));
            Room.SendMessage(new UpdateStackMapMessageComposer(Room, Item.GetAffectedTiles));
        }
    }
}

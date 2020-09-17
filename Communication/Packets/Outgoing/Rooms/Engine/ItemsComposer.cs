
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemsComposer : MessageComposer
    {
        public Item[] Objects { get; }
        public int OwnerId { get; }
        public string OwnerName { get; }

        public ItemsComposer(Item[] Objects, Room Room)
            : base(Composers.ItemsMessageComposer)
        {
            this.Objects = Objects;
            this.OwnerId = Room.RoomData.OwnerId;
            this.OwnerName = Room.RoomData.OwnerName;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(OwnerId);
            packet.WriteString(OwnerName);

            packet.WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                WriteWallItem(Item, OwnerId, packet);
            }
        }

        private void WriteWallItem(Item Item, int UserId, Composer packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteInteger(Item.Data.SpriteId);

            try
            {
                packet.WriteString(Item.wallCoord);
            }
            catch
            {
                packet.WriteString("");
            }

            ItemBehaviourUtility.GenerateWallExtradata(Item, packet);

            packet.WriteInteger(-1);
            packet.WriteInteger((Item.Data.Modes > 1) ? 1 : 0);
            packet.WriteInteger(UserId);
        }
    }
}

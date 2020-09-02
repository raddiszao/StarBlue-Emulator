using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemAddComposer : ServerPacket
    {
        public ItemAddComposer(Item Item)
            : base(ServerPacketHeader.ItemAddMessageComposer)
        {
            base.WriteString(Item.Id.ToString());
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteString(Item.wallCoord != null ? Item.wallCoord : string.Empty);

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            base.WriteInteger(-1);
            base.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0); // Type New R63 ('use bottom')
            base.WriteInteger(Item.UserID);
            base.WriteString(Item.Username);
        }
    }
}
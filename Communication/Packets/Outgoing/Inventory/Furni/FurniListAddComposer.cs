﻿
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListAddComposer : MessageComposer
    {
        public Item Item { get; }

        public FurniListAddComposer(Item Item)
            : base(Composers.FurniListAddMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Item.Id);
            packet.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            packet.WriteInteger(Item.Id);
            packet.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                packet.WriteInteger(1);
                packet.WriteInteger(256);
                packet.WriteString(Item.ExtraData);
                packet.WriteInteger(Item.LimitedNo);
                packet.WriteInteger(Item.LimitedTot);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, packet);
            }

            packet.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            packet.WriteBoolean(Item.GetBaseItem().AllowTrade);
            packet.WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            packet.WriteBoolean(ItemUtility.IsRare(Item));
            packet.WriteInteger(-1);//Seconds to expiration.
            packet.WriteBoolean(true);
            packet.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                packet.WriteString(string.Empty);
                packet.WriteInteger(0);
            }
        }
    }
}

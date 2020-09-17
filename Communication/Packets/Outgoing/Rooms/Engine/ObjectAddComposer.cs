using StarBlue.HabboHotel.Items;
using StarBlue.Utilities;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectAddComposer : MessageComposer
    {
        public Item Item { get; }

        public ObjectAddComposer(Item Item)
            : base(Composers.ObjectAddMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Item.Id);
            packet.WriteInteger(Item.GetBaseItem().SpriteId);
            packet.WriteInteger(Item.GetX);
            packet.WriteInteger(Item.GetY);
            packet.WriteInteger(Item.Rotation);
            packet.WriteString(string.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            packet.WriteString(string.Empty);

            if (Item.LimitedNo > 0)
            {
                packet.WriteInteger(1);
                packet.WriteInteger(256);
                packet.WriteString(Item.ExtraData);
                packet.WriteInteger(Item.LimitedNo);
                packet.WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.INFO_TERMINAL || Item.Data.InteractionType == InteractionType.ROOM_PROVIDER)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(1);
                packet.WriteInteger(1);
                packet.WriteString("internalLink");
                packet.WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(1);
                packet.WriteInteger(1);
                packet.WriteString("effectId");
                packet.WriteString(Item.ExtraData);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATA)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString("6");
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }

                packet.WriteInteger(100);
            }

            else if (Item.Data.InteractionType == InteractionType.PLANT_SEED)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }
                packet.WriteInteger(12);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString("0");
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }

                packet.WriteInteger(1);
            }

            else if (Item.Data.InteractionType == InteractionType.EASTEREGG)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }
                packet.WriteInteger(20);
            }

            else if (Item.Data.InteractionType == InteractionType.MAGICEGG)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }
                packet.WriteInteger(23);
            }
            else if (Item.Data.InteractionType == InteractionType.CASHIER)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(7);
                packet.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(int.Parse(Item.ExtraData));
                }
                packet.WriteInteger(1);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, packet);
            }

            packet.WriteInteger(-1); // to-do: check
            packet.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            packet.WriteInteger(Item.UserID);
            packet.WriteString(Item.Username);
        }
    }
}
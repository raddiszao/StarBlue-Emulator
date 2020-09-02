using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectsComposer : ServerPacket
    {
        public ObjectsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ObjectsMessageComposer)
        {
            List<Item> ItemsTemp = Objects.ToList();
            base.WriteInteger(1);

            base.WriteInteger(Room.RoomData.OwnerId);
            base.WriteString(Room.RoomData.OwnerName);

            if (Objects.Length > 2500)
            {
                base.WriteInteger(2500);
                for (int i = 0; i < 2500; i++)
                {
                    Item Item = Objects[i];
                    ItemsTemp.Remove(Item);
                    WriteFloorItem(Item, Convert.ToInt32(Item.UserID));
                }

                Room.SendMessage(new ObjectsComposer(ItemsTemp.ToArray(), Room));
            }
            else
            {
                base.WriteInteger(Objects.Length);
                foreach (Item Item in Objects)
                {
                    WriteFloorItem(Item, Convert.ToInt32(Item.UserID));
                }
            }
        }

        private void WriteFloorItem(Item Item, int UserID)
        {
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteInteger(Item.GetX);
            base.WriteInteger(Item.GetY);
            base.WriteInteger(Item.Rotation);
            base.WriteString(string.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            base.WriteString(string.Empty);

            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(1);
                base.WriteInteger(256);
                base.WriteString(Item.ExtraData);
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.INFO_TERMINAL || Item.Data.InteractionType == InteractionType.ROOM_PROVIDER)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("internalLink");
                base.WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("effectId");
                base.WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.PINATA)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString("6");
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }

                base.WriteInteger(100);
            }

            else if (Item.Data.InteractionType == InteractionType.PLANT_SEED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(12);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString("0");
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }

                base.WriteInteger(1);
            }

            else if (Item.Data.InteractionType == InteractionType.EASTEREGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(20);
            }

            else if (Item.Data.InteractionType == InteractionType.MAGICEGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(23);
            }
            else if (Item.Data.InteractionType == InteractionType.CASHIER)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(1);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            base.WriteInteger(-1); // to-do: check
            if (Item.Data.InteractionType == InteractionType.TELEPORT || Item.Data.InteractionType == InteractionType.VENDING_MACHINE)
            {
                base.WriteInteger(2);
            }
            else if (Item.GetBaseItem().Modes > 1)
            {
                base.WriteInteger(1);
            }
            else
            {
                base.WriteInteger(0);
            }

            base.WriteInteger(UserID);
        }
    }
}
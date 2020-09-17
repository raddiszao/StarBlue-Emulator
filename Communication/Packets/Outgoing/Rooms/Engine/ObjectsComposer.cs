using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectsComposer : MessageComposer
    {
        public Item[] Objects { get; }
        public int OwnerId { get; }
        public string OwnerName { get; }

        public Room Room { get; }

        public ObjectsComposer(Item[] Objects, Room Room)
            : base(Composers.ObjectsMessageComposer)
        {
            this.Objects = Objects;
            this.OwnerId = Room.RoomData.OwnerId;
            this.OwnerName = Room.RoomData.OwnerName;
            this.Room = Room;
        }

        public override void Compose(Composer packet)
        {
            List<Item> ItemsTemp = Objects.ToList();
            packet.WriteInteger(1);

            packet.WriteInteger(OwnerId);
            packet.WriteString(OwnerName);

            if (Objects.Length > 2500)
            {
                packet.WriteInteger(2500);
                for (int i = 0; i < 2500; i++)
                {
                    Item Item = Objects[i];
                    ItemsTemp.Remove(Item);
                    WriteFloorItem(Item, Convert.ToInt32(Item.UserID), packet);
                }

                Room.SendMessage(new ObjectsComposer(ItemsTemp.ToArray(), Room));
            }
            else
            {
                packet.WriteInteger(Objects.Length);
                foreach (Item Item in Objects)
                {
                    WriteFloorItem(Item, Convert.ToInt32(Item.UserID), packet);
                }
            }
        }

        private void WriteFloorItem(Item Item, int UserID, Composer packet)
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
            if (Item.Data.InteractionType == InteractionType.TELEPORT || Item.Data.InteractionType == InteractionType.VENDING_MACHINE)
            {
                packet.WriteInteger(2);
            }
            else if (Item.GetBaseItem().Modes > 1)
            {
                packet.WriteInteger(1);
            }
            else
            {
                packet.WriteInteger(0);
            }

            packet.WriteInteger(UserID);
        }
    }
}
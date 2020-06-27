using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    class FurniHasNoFurniBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.ConditionFurniHasNoFurni; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public FurniHasNoFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Option = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            BoolData = Option == 1;

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }
        }

        public bool Execute(params object[] Params)
        {
            return BoolData ? AllFurniHaveNotFurniOn() : SomeFurniHaveNotFurniOn();
        }

        public bool AllFurniHaveNotFurniOn()
        {
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                {
                    continue;
                }

                bool NoFurni = false;
                List<Item> Items = Instance.GetGameMap().GetAllRoomItemForSquare(Item.GetX, Item.GetY);
                if (!(Items.Where(x => x.GetZ >= Item.GetZ).Count() > 1))
                {
                    NoFurni = true;
                }

                if (!NoFurni)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SomeFurniHaveNotFurniOn()
        {
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item)) //Si el Furni esta en la sala
                {
                    continue;
                }

                bool NoFurni = false;
                foreach (String I in ItemsData.Split(';'))
                {
                    if (String.IsNullOrEmpty(I))
                    {
                        continue;
                    }

                    Item II = Instance.GetRoomItemHandler().GetItem(Convert.ToInt32(I));

                    if (II == null)
                    {
                        continue;
                    }

                    List<Item> Items = Instance.GetGameMap().GetAllRoomItemForSquare(II.GetX, II.GetY);
                    if (!(Items.Where(x => x.GetZ >= Item.GetZ).Count() > 1))
                    {
                        NoFurni = true;
                        break;
                    }

                }
                if (!NoFurni)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
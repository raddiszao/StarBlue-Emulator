using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class FurniDoesntMatchStateAndPositionBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.ConditionDontMatchStateAndPosition;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public FurniDoesntMatchStateAndPositionBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            int State = Packet.PopInt();
            int Direction = Packet.PopInt();
            int Placement = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }

            StringData = State + ";" + Direction + ";" + Placement;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(StringData) || StringData == "0;0;0" || SetItems.Count == 0)
            {
                return false;
            }

            foreach (Item Item in SetItems.Values.ToList())
            {
                if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                {
                    continue;
                }

                foreach (string I in ItemsData.Split(';'))
                {
                    if (string.IsNullOrEmpty(I))
                    {
                        continue;
                    }

                    Item II = Instance.GetRoomItemHandler().GetItem(Convert.ToInt32(I.Split(':')[0]));
                    if (II == null)
                    {
                        continue;
                    }

                    string[] partsString = I.Split(':');
                    string[] part = partsString[1].Split(',');

                    if (int.Parse(StringData.Split(';')[0]) == 1)//State
                    {
                        if (II.ExtraData == part[4].ToString())
                        {
                            return false;
                        }
                    }

                    if (int.Parse(StringData.Split(';')[1]) == 1)//Direction
                    {
                        if (II.Rotation == Convert.ToInt32(part[3]))
                        {
                            return false;
                        }
                    }

                    if (int.Parse(StringData.Split(';')[2]) == 1)//Position
                    {
                        if (II.GetX == Convert.ToInt32(part[0]) && II.GetY == Convert.ToInt32(part[1]) && II.GetZ == Convert.ToDouble(part[2]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
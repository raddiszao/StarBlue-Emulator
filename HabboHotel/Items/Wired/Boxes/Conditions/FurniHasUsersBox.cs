﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    class FurniHasUsersBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.ConditionFurniHasUsers;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public FurniHasUsersBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

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
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                {
                    continue;
                }

                bool HasUsers = false;
                foreach (ThreeDCoord Tile in Item.GetAffectedTiles.Values)
                {
                    if (Instance.GetGameMap().SquareHasUsers(Tile.X, Tile.Y))
                    {
                        HasUsers = true;
                    }
                }

                if (Instance.GetGameMap().SquareHasUsers(Item.GetX, Item.GetY))
                {
                    HasUsers = true;
                }

                if (!HasUsers)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
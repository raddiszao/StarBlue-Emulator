﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class UserCountDoesntInRoomBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.ConditionUserCountDoesntInRoom;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public UserCountDoesntInRoomBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            int CountOne = Packet.PopInt();
            int CountTwo = Packet.PopInt();

            StringData = CountOne + ";" + CountTwo;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            int CountOne = StringData != null ? int.Parse(StringData.Split(';')[0]) : 1;
            int CountTwo = StringData != null ? int.Parse(StringData.Split(';')[1]) : 50;

            if (Instance.UserCount >= CountOne && Instance.UserCount <= CountTwo)
            {
                return false;
            }

            return true;
        }
    }
}
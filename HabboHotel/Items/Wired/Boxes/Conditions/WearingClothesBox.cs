﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    class WearingClothesBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.ConditionWearingClothes; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public WearingClothesBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Look = Packet.PopString();

            StringData = Look;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(StringData))
            {
                return false;
            }

            string Look = StringData;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            if (User.GetClient().GetHabbo().Look == Look)
            { return true; }
            else { return false; }
        }
    }
}
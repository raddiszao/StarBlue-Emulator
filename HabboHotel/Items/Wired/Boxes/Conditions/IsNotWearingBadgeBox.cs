﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Badges;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    class IsNotWearingBadgeBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.ConditionIsWearingBadge;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public IsNotWearingBadgeBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BadgeCode = Packet.PopString();

            StringData = BadgeCode;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(StringData))
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
            {
                return false;
            }

            if (!Player.GetBadgeComponent().GetBadges().Contains(Player.GetBadgeComponent().GetBadge(StringData)))
            {
                return true;
            }

            foreach (Badge Badge in Player.GetBadgeComponent().GetBadges().ToList())
            {
                if (Badge.Slot <= 0)
                {
                    continue;
                }

                if (Badge.Code == StringData)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
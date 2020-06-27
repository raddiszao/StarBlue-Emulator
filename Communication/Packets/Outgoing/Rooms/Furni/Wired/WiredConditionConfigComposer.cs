﻿using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class WiredConditionConfigComposer : ServerPacket
    {
        public WiredConditionConfigComposer(IWiredItem Box)
            : base(ServerPacketHeader.WiredConditionConfigMessageComposer)
        {
            base.WriteBoolean(false);
            if (Box.Type == WiredBoxType.TotalUsersCoincidence) { base.WriteInteger(25); }
            else
            {
                base.WriteInteger(15);
            }

            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                base.WriteInteger(Item.Id);
            }

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);
            base.WriteString(Box.StringData);

            if (Box.Type == WiredBoxType.ConditionDateRangeActive)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                base.WriteInteger(2);//Loop
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);

            }

            if (Box.Type == WiredBoxType.ConditionMatchStateAndPosition || Box.Type == WiredBoxType.ConditionDontMatchStateAndPosition)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0;0";
                }

                base.WriteInteger(3);//Loop
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.ConditionUserCountInRoom || Box.Type == WiredBoxType.ConditionUserCountDoesntInRoom)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                base.WriteInteger(2);//Loop
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 1);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 50);
            }

            if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni || Box.Type == WiredBoxType.ConditionFurniHasFurni)
            {
                base.WriteInteger(1);
            }

            if (Box.Type != WiredBoxType.ConditionUserCountInRoom && Box.Type != WiredBoxType.ConditionUserCountDoesntInRoom && Box.Type != WiredBoxType.ConditionFurniHasNoFurni && Box.Type != WiredBoxType.ConditionFurniHasFurni && Box.Type != WiredBoxType.ConditionDateRangeActive)
            {
                base.WriteInteger(0);
            }
            else if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni || Box.Type == WiredBoxType.ConditionFurniHasFurni)
            {
                base.WriteInteger(Box.BoolData ? 1 : 0);
            }
            base.WriteInteger(0);
            base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
        }
    }
}
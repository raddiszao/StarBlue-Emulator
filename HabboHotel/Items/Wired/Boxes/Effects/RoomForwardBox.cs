﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class RoomForwardBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectRoomForward;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public RoomForwardBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Room = Packet.PopString();
            StringData = int.Parse(Room) + "";
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
            {
                return false;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(StringData))
            {
                return false;
            }

            User.GetClient().SendMessage(new CloseConnectionComposer());
            User.GetClient().SendMessage(new RoomForwardComposer(int.Parse(StringData)));

            return true;
        }
    }
}
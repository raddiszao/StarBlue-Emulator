﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotFollowsUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotFollowsUserBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public BotFollowsUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            int FollowMode = Packet.PopInt();//1 = follow, 0 = don't.
            string BotConfiguration = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            StringData = FollowMode + ";" + BotConfiguration;
        }

        public bool OnCycle()
        {
            if (_queue.Count == 0)
            {
                _queue.Clear();
                TickCount = Delay;
                return true;
            }

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                {
                    continue;
                }

                BotFollowsUser(Player);
            }

            TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
            {
                return false;
            }

            TickCount = Delay;
            _queue.Enqueue(Player);
            return true;
        }

        public bool BotFollowsUser(Habbo Player)
        {
            if (string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            if (Player == null)
            {
                return false;
            }

            RoomUser Human = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (Human == null)
            {
                return false;
            }

            string[] Stuff = StringData.Split(';');
            if (Stuff.Length != 2)
            {
                return false;//This is important, incase a cunt scripts.
            }

            string Username = Stuff[1];

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(Username);
            if (User == null)
            {
                return false;
            }

            if (!int.TryParse(Stuff[0], out int FollowMode))
            {
                return false;
            }

            if (FollowMode == 0)
            {
                User.BotData.ForcedUserTargetMovement = 0;

                if (User.IsWalking)
                {
                    User.ClearMovement(true);
                }
            }
            else if (FollowMode == 1)
            {
                User.BotData.ForcedUserTargetMovement = Player.Id;

                if (User.IsWalking)
                {
                    User.ClearMovement(true);
                }

                User.MoveTo(Human.X, Human.Y);

                if (Gamemap.TileDistance(Human.X, Human.Y, User.X, User.Y) <= 1)
                {
                    Instance.GetWired().TriggerEvent(WiredBoxType.TriggerBotReachedAvatar, true);
                }
            }

            return true;
        }
    }
}
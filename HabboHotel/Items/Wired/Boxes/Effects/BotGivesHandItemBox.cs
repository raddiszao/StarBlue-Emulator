﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotGivesHandItemBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotGivesHanditemBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotGivesHandItemBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int DrinkID = Packet.PopInt();
            string BotName = Packet.PopString();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            StringData = BotName.ToString() + ";" + DrinkID.ToString();
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

            Habbo Player = (Habbo)Params[0];

            if (Player == null)
            {
                return false;
            }

            RoomUser Actor = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);

            if (Actor == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(StringData.Split(';')[0]);

            if (User == null)
            {
                return false;
            }

            if (User.BotData.TargetUser == 0)
            {
                if (!Instance.GetGameMap().CanWalk(Actor.SquareBehind.X, Actor.SquareBehind.Y, false))
                {
                    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Não consigo alcançar, chegue mais perto.", 0, 31));
                }
                else
                {
                    string[] Data = StringData.Split(';');


                    if (!int.TryParse(Data[1], out int DrinkId))
                    {
                        return false;
                    }

                    User.CarryItem(DrinkId);
                    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Aqui está sua bebida, " + Player.GetClient().GetHabbo().Username + "!", 0, 31));

                    User.MoveTo(Actor.SquareBehind.X, Actor.SquareBehind.Y);
                }
            }
            return true;
        }
    }
}

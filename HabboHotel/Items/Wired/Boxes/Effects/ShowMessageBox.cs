using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class ShowMessageBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectShowMessage;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public string Message2 { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public ShowMessageBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();
            int Unused = Packet.PopInt();
            Delay = Packet.PopInt();

            StringData = Message;
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

                SendMessageToUser(Player);
            }

            TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
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

        private void SendMessageToUser(Habbo Player)
        {
            if (Player == null || StringData == "")
            {
                return;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return;
            }

            string Message = StringData;
            string MessageFiltered = StringData;

            // Convertir esta chapuza integral a switch

            // USER VARIABLES
            if (StringData.Contains("%user%"))
            {
                MessageFiltered = MessageFiltered.Replace("%user%", Player.Username);
            }

            if (StringData.Contains("%userid%"))
            {
                MessageFiltered = MessageFiltered.Replace("%userid%", Convert.ToString(Player.Id));
            }

            if (StringData.Contains("%credits%"))
            {
                MessageFiltered = MessageFiltered.Replace("%honor%", Convert.ToString(Player.Credits));
            }

            if (StringData.Contains("%gotw%"))
            {
                MessageFiltered = MessageFiltered.Replace("%gotw%", Convert.ToString(Player.GOTWPoints));
            }

            if (StringData.Contains("%duckets%"))
            {
                MessageFiltered = MessageFiltered.Replace("%duckets%", Convert.ToString(Player.Duckets));
            }

            if (StringData.Contains("%diamonds%"))
            {
                MessageFiltered = MessageFiltered.Replace("%diamonds%", Convert.ToString(Player.Diamonds));
            }

            if (StringData.Contains("%rank%"))
            {
                MessageFiltered = MessageFiltered.Replace("%rank%", Convert.ToString(Player.Rank));
            }

            if (StringData.Contains("%events%"))
            {
                MessageFiltered = MessageFiltered.Replace("%events%", Convert.ToString(Player.UserPoints));
            }

            // ROOM VARIABLES
            if (StringData.Contains("%roomname%"))
            {
                MessageFiltered = MessageFiltered.Replace("%roomname%", Player.CurrentRoom.RoomData.Name);
            }

            if (StringData.Contains("%roomusers%"))
            {
                MessageFiltered = MessageFiltered.Replace("%roomusers%", Player.CurrentRoom.UserCount.ToString());
            }

            if (StringData.Contains("%roomowner%"))
            {
                MessageFiltered = MessageFiltered.Replace("%roomowner%", Player.CurrentRoom.RoomData.OwnerName.ToString());
            }

            if (StringData.Contains("%roomlikes%"))
            {
                MessageFiltered = MessageFiltered.Replace("%roomlikes%", Player.CurrentRoom.RoomData.Score.ToString());
            }

            // HOTEL VARIABLES
            if (StringData.Contains("%userson%"))
            {
                MessageFiltered = MessageFiltered.Replace("%userson%", StarBlueServer.GetGame().GetClientManager().Count.ToString());
            }

            // SPECIAL VARIABLES
            if (StringData.Contains("%sit%"))
            {
                Message = Message.Replace("%sit%", "Está sentado.");

                if (User.Statusses.ContainsKey("lie") || User.isLying || User.RidingHorse || User.IsWalking)
                {
                    return;
                }

                if (!User.Statusses.ContainsKey("sit"))
                {
                    if ((User.RotBody % 2) == 0)
                    {
                        if (User == null)
                        {
                            return;
                        }

                        try
                        {
                            User.Statusses.Add("sit", "1.0");
                            User.Z -= 0.35;
                            User.isSitting = true;
                            User.UpdateNeeded = true;
                        }
                        catch { }
                    }
                    else
                    {
                        User.RotBody--;
                        User.Statusses.Add("sit", "1.0");
                        User.Z -= 0.35;
                        User.isSitting = true;
                        User.UpdateNeeded = true;
                    }
                }
                else if (User.isSitting == true)
                {
                    User.Z += 0.35;
                    User.Statusses.Remove("sit");
                    User.Statusses.Remove("1.0");
                    User.isSitting = false;
                    User.UpdateNeeded = true;
                }
            }

            if (StringData.Contains("%stand%"))
            {
                Message = Message.Replace("%stand%", "Você levantou");
                if (User.isSitting)
                {
                    User.Statusses.Remove("sit");
                    User.Z += 0.35;
                    User.isSitting = false;
                    User.UpdateNeeded = true;
                }
                else if (User.isLying)
                {
                    User.Statusses.Remove("lay");
                    User.Z += 0.35;
                    User.isLying = false;
                    User.UpdateNeeded = true;
                }
            }

            if (StringData.Contains("%lay%"))
            {
                Message = Message.Replace("%lay%", "Está deitado");

                Room Room = Player.GetClient().GetHabbo().CurrentRoom;

                if (!Room.GetGameMap().ValidTile(User.X + 2, User.Y + 2) && !Room.GetGameMap().ValidTile(User.X + 1, User.Y + 1))
                {
                    Player.GetClient().SendWhisper("Opa, você não pode ir para a cama aqui!");
                    return;
                }

                if (User.Statusses.ContainsKey("sit") || User.isSitting || User.RidingHorse || User.IsWalking)
                {
                    return;
                }

                if (Player.GetClient().GetHabbo().Effects().CurrentEffect > 0)
                {
                    Player.GetClient().GetHabbo().Effects().ApplyEffect(0);
                }

                if (!User.Statusses.ContainsKey("lay"))
                {
                    if ((User.RotBody % 2) == 0)
                    {
                        if (User == null)
                        {
                            return;
                        }

                        try
                        {
                            User.Statusses.Add("lay", "1.0 null");
                            User.Z -= 0.35;
                            User.isLying = true;
                            User.UpdateNeeded = true;
                        }
                        catch { }
                    }
                    else
                    {
                        User.RotBody--;//
                        User.Statusses.Add("lay", "1.0 null");
                        User.Z -= 0.35;
                        User.isLying = true;
                        User.UpdateNeeded = true;
                    }

                }
                else
                {
                    User.Z += 0.35;
                    User.Statusses.Remove("lay");
                    User.Statusses.Remove("1.0");
                    User.isLying = false;
                    User.UpdateNeeded = true;
                }
            }

            Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, MessageFiltered, 0, 34));
        }
    }
}
﻿using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using System;
using System.Collections.Generic;
using System.Data;
namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Bots
{
    internal class SaveBotActionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();
            string DataString = Packet.PopString();
            if (ActionId < 1 || ActionId > 5)
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser Bot))
            {
                return;
            }

            if ((Bot.BotData.ownerID != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("bot_edit_any_override")))
            {
                return;
            }

            RoomBot RoomBot = Bot.BotData;
            if (RoomBot == null)
            {
                return;
            }
            /* 1 = Copy looks
* 2 = Setup Speech
* 3 = Relax
* 4 = Dance
* 5 = Change Name
*/

            switch (ActionId)
            {
                #region Copy Looks (1)
                case 1:
                    {
                        ServerPacket UserChangeComposer = new ServerPacket(ServerPacketHeader.UserChangeMessageComposer);
                        UserChangeComposer.WriteInteger(Bot.VirtualId);
                        UserChangeComposer.WriteString(Session.GetHabbo().Look);
                        UserChangeComposer.WriteString(Session.GetHabbo().Gender);
                        UserChangeComposer.WriteString(Bot.BotData.Motto);
                        UserChangeComposer.WriteInteger(0);
                        Room.SendMessage(UserChangeComposer);
                        //Change the defaults
                        Bot.BotData.Look = Session.GetHabbo().Look;
                        Bot.BotData.Gender = Session.GetHabbo().Gender;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + Session.GetHabbo().Gender + "' WHERE `id` = '" + Bot.BotData.Id + "' LIMIT 1");
                            dbClient.AddParameter("look", Session.GetHabbo().Look);
                            dbClient.RunQuery();
                        }
                        //Room.SendMessage(new UserChangeComposer(BotUser.GetClient(), true));
                        break;
                    }
                #endregion
                #region Setup Speech (2)
                case 2:
                    {
                        string[] ConfigData = DataString.Split(new string[]
                        {
                            ";#;"
                        }, StringSplitOptions.None);
                        string[] SpeechData = ConfigData[0].Split(new char[]
                        {
                            '\r',
                            '\n'
                        }, StringSplitOptions.RemoveEmptyEntries);

                        if (ConfigData.Length < 2)
                        {
                            Session.SendNotification("Ocorreu um erro ao salvar as configurações do bot, tente novamente.");
                            return;
                        }

                        string AutomaticChat = Convert.ToString(ConfigData[1]);
                        string SpeakingInterval = Convert.ToString(ConfigData[2]);
                        string MixChat = Convert.ToString(ConfigData[3]);
                        if (string.IsNullOrEmpty(SpeakingInterval) || Convert.ToInt32(SpeakingInterval) <= 0 || Convert.ToInt32(SpeakingInterval) < 7)
                        {
                            SpeakingInterval = "7";
                        }

                        RoomBot.AutomaticChat = Convert.ToBoolean(AutomaticChat);
                        RoomBot.SpeakingInterval = Convert.ToInt32(SpeakingInterval);
                        RoomBot.MixSentences = Convert.ToBoolean(MixChat);
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("DELETE FROM `bots_speech` WHERE `bot_id` = '" + Bot.BotData.Id + "'");
                            #region Save Data - TODO: MAKE METHODS FOR THIS.  
                            for (int i = 0; i <= SpeechData.Length - 1; i++)
                            {
                                dbClient.SetQuery("INSERT INTO `bots_speech` (`bot_id`, `text`) VALUES (@id, @data)");
                                dbClient.AddParameter("id", BotId);
                                dbClient.AddParameter("data", SpeechData[i]);
                                dbClient.RunQuery();
                                dbClient.SetQuery("UPDATE `bots` SET `automatic_chat` = @AutomaticChat, `speaking_interval` = @SpeakingInterval, `mix_sentences` = @MixChat WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("id", BotId);
                                dbClient.AddParameter("AutomaticChat", AutomaticChat.ToLower());
                                dbClient.AddParameter("SpeakingInterval", Convert.ToInt32(SpeakingInterval));
                                dbClient.AddParameter("MixChat", StarBlueServer.BoolToEnum(Convert.ToBoolean(MixChat)));
                                dbClient.RunQuery();
                            }
                            #endregion
                            #region Handle Speech
                            RoomBot.RandomSpeech.Clear();
                            dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @id");
                            dbClient.AddParameter("id", BotId);
                            DataTable BotSpeech = dbClient.GetTable();
                            List<RandomSpeech> Speeches = new List<RandomSpeech>();
                            foreach (DataRow Speech in BotSpeech.Rows)
                            {
                                RoomBot.RandomSpeech.Add(new RandomSpeech(Convert.ToString(Speech["text"]), BotId));
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Relax (3)
                case 3:
                    {
                        if (Bot.BotData.WalkingMode == "stand")
                        {
                            Bot.BotData.WalkingMode = "freeroam";
                        }
                        else
                        {
                            Bot.BotData.WalkingMode = "stand";
                        }

                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `bots` SET `walk_mode` = '" + Bot.BotData.WalkingMode + "' WHERE `id` = '" + Bot.BotData.Id + "' LIMIT 1");
                        }
                        break;
                    }
                #endregion
                #region Dance (4)
                case 4:
                    {
                        Bot.ApplyEffect(0);
                        if (Bot.BotData.DanceId > 0)
                        {
                            Bot.BotData.DanceId = 0;
                            //Bot.ApplyEffect(187);
                        }
                        else
                        {
                            Random RandomDance = new Random();
                            Bot.BotData.DanceId = RandomDance.Next(1, 5);
                        }

                        Room.SendMessage(new DanceComposer(Bot, Bot.BotData.DanceId));
                        break;
                    }
                #endregion
                #region Change Name (5)
                case 5:
                    {
                        if (DataString.Length == 0)
                        {
                            Session.SendWhisper("Por favor, colócale un nombre al robot.");
                            return;
                        }
                        else if (DataString.Length >= 16)
                        {
                            Session.SendWhisper("O nome do bot não pode exceder 16 caracteres.");
                            return;
                        }
                        if (DataString.Contains("<img src") || DataString.Contains("<font ") || DataString.Contains("</font>") || DataString.Contains("</a>") || DataString.Contains("<i>"))
                        {
                            Session.SendWhisper("O que você está tentando fazer?");
                            return;
                        }
                        Bot.BotData.Name = DataString;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bots` SET `name` = @name WHERE `id` = '" + Bot.BotData.Id + "' LIMIT 1");
                            dbClient.AddParameter("name", DataString);
                            dbClient.RunQuery();
                        }
                        Room.SendMessage(new UsersComposer(Bot));
                        break;
                    }
                    #endregion
            }
        }
    }
}
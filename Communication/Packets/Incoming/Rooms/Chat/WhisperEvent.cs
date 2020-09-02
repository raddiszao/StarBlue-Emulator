using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Chat.Styles;
using StarBlue.HabboHotel.Users;
using StarBlue.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Chat
{
    public class WhisperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
            {
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Este quarto está mutado.", 34);
                return;
            }

            if (StarBlueServer.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
            {
                return;
            }

            string Params = Packet.PopString();
            string ToUser = Params.Split(' ')[0];
            string Message = Params.Substring(ToUser.Length + 1);
            int Colour = Packet.PopInt();

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(ToUser);
            if (User2 == null && ToUser != "group_whisper")
            {
                return;
            }

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            if (!StarBlueServer.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Colour = 0;
            }

            if (Message.StartsWith("@") && Message.Split(' ').Length >= 1)
            {
                string[] Params2 = Message.Split(' ');
                string To = Params2[0].Split('@')[1];

                ServerPacket MentionPacket = new MentionUserComposer(Room.RoomData.Name, Room.RoomData.Id, Message, Session.GetHabbo().Username, Session.GetHabbo().Look);

                if (Session.GetHabbo().Rank >= 14 && (To == "everyone" || To == "here"))
                {
                    StarBlueServer.GetGame().GetWebClientManager().SendMessage(MentionPacket);
                }
                else
                {
                    Habbo UserHabboMentioned = StarBlueServer.GetHabboByUsername(To);
                    if (UserHabboMentioned == null || UserHabboMentioned.GetClient() == null)
                    {
                        Session.SendWhisper("Não foi possível encontrar este usuário.", 34);
                    }
                    else
                    {
                        if (UserHabboMentioned.Username == Session.GetHabbo().Username)
                        {
                            Session.SendWhisper("Não pode mencionar você mesmo.", 34);
                        }
                        else if (UserHabboMentioned.DisabledMentions && Session.GetHabbo().Rank < 16)
                        {
                            Session.SendWhisper("Este usuário desabilitou as menções.", 34);
                        }
                        else
                        {
                            if (UserHabboMentioned.SendWebPacket(MentionPacket))
                            {
                                Session.SendWhisper("Você mencionou o usuário " + UserHabboMentioned.GetClient().GetHabbo().Username + ".", 34);
                            }
                            else
                            {
                                UserHabboMentioned.GetClient().SendMessage(RoomNotificationComposer.SendBubble("advice", Session.GetHabbo().Username + " mencionou você: " + Message, "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                                Session.SendWhisper("Você mencionou o usuário " + UserHabboMentioned.GetClient().GetHabbo().Username + ".", 34);
                            }
                        }
                    }
                }
            }

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (User.IncrementAndCheckFlood(out int MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }

            if (Room.MutedUsers.ContainsKey(Session.GetHabbo().Id))
            {
                if (Room.MutedUsers[Session.GetHabbo().Id] < StarBlueServer.GetUnixTimestamp())
                {
                    Room.MutedUsers.Remove(Session.GetHabbo().Id);
                }
                else
                {
                    Session.SendWhisper("Oops, você está silenciad@.", 34);
                    return;
                }
            }

            if (ToUser.Equals("group_whisper"))
            {
                if (Room.MultiWhispers.Count > 0)
                {
                    if (Room.MultiWhispers.Contains(User))
                    {
                        foreach (RoomUser RoomUserMW in Room.MultiWhispers)
                        {
                            if (RoomUserMW != null && RoomUserMW.GetClient() != null && RoomUserMW.GetClient().GetRoomUser() != null)
                            {
                                RoomUserMW.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "@blue@ Sussurrando com (" + string.Join(",", Room.MultiWhispers.Select(r => r.GetClient().GetHabbo().Username)) + "): " + Message, 0, User.LastBubble));
                            }
                        }
                    }
                    else
                    {
                        Session.SendWhisper("Você não participa de nenhum grupo de sussurro.", 34);
                        return;
                    }
                }

                return;
            }

            if (User2 != null && User2.GetClient() != null && !User2.GetClient().GetHabbo().ReceiveWhispers && !Session.GetHabbo().GetPermissions().HasRight("room_whisper_override"))
            {
                Session.SendWhisper("Oops, este usuário não permite sussurros.");
                return;
            }

            //     if (Session.GetHabbo().LastMessage == Message)
            //     {
            //         Session.GetHabbo().LastMessageCount++;
            //          if (Session.GetHabbo().LastMessageCount > 3)
            //        {
            //              StarBlueServer.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Usuário: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Vezes: " + Session.GetHabbo().LastMessageCount + "."));
            //               Session.GetHabbo().LastMessageCount = 0;
            //            }
            //        }

            Room.GetFilter().CheckMessage(Message);

            StarBlueServer.GetGame().GetChatManager().GetLogs().StoreChatlog(new StarBlue.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(Session.GetHabbo().Id, Room.Id, ": " + Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room.RoomData));

            Room.AddChatlog(Session.GetHabbo().Id, ": " + Message);

            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out string word))
            {
                Session.GetHabbo().BannedPhraseCount++;

                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("Você mencionou uma palavra inadequada para o hotel! Notificação " + Session.GetHabbo().BannedPhraseCount + "/10");

                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(StarBlueServer.GetUnixTimestamp()).ToLocalTime();

                    StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    StarBlueServer.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de divulgador:",
                    "<b><font color=\"#B40404\">Lembre-se de investigar cuidadosamente antes de recorrer a uma sanção.</font></b><br><br>Palavra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Sequência:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    StarBlueServer.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer Spam con la Frase (" + Message + ")", (StarBlueServer.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }

                Session.SendMessage(new WhisperComposer(User.VirtualId, "Mensagem inapropriada.", 0, User.LastBubble));
                return;
            }

            Session.GetHabbo().LastMessage = Message;
            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

            User.UnIdle();
            User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));

            if (User2 != null && !User2.IsBot && User2.UserId != User.UserId)
            {
                if (!User2.GetClient().GetHabbo().GetIgnores().IgnoredUserIds().Contains(Session.GetHabbo().Id))
                {
                    User2.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));
                }
            }

            List<RoomUser> ToNotify = Room.GetRoomUserManager().GetRoomUserByRank(7);
            if (ToNotify.Count > 0)
            {
                foreach (RoomUser user in ToNotify)
                {
                    if (user != null && user.HabboId != User2.HabboId && user.HabboId != User.HabboId)
                    {
                        //if (Session.GetHabbo().Rank > user.GetClient().GetHabbo().Rank)
                        //  continue;

                        if (user.GetClient() != null && user.GetClient().GetHabbo() != null && !user.GetClient().GetHabbo().IgnorePublicWhispers)
                        {
                            user.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "@red@ [Sussurando com " + ToUser + "] " + Message, 0, User.LastBubble));
                        }
                    }
                }
            }
        }
    }
}
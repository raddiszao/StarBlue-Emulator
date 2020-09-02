using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Chat.Logs;
using StarBlue.HabboHotel.Rooms.Chat.Styles;
using StarBlue.Utilities;
using System;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Chat
{
    public class ChatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
            {
                return;
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
            {
                Message = Message.Substring(0, 100);
            }

            int Colour = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Colour = 0;
            }

            User.UnIdle();

            if (StarBlueServer.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
            {
                return;
            }

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            if (!Room.CheckRights(Session, false) && Room.muteSignalEnabled == true)
            {
                Session.SendWhisper("Este quarto está mutado.", 34);
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Este quarto está mutado.", 34);
                return;
            }

            if (Room.MutedUsers.ContainsKey(Session.GetHabbo().Id))
            {
                if (Room.MutedUsers[Session.GetHabbo().Id] < StarBlueServer.GetUnixTimestamp())
                {
                    Room.MutedUsers.Remove(Session.GetHabbo().Id);
                }
                else
                {
                    Session.SendWhisper("Oops, você está silenciad@!", 34);
                    return;
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

            Message = Room.GetFilter().CheckMessage(Message);

            if (Message.StartsWith(":", StringComparison.CurrentCulture) && StarBlueServer.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
            {
                return;
            }

            //if (Session.GetHabbo().LastMessage == Message)
            //  {
            //      Session.GetHabbo().LastMessageCount++;
            //      if (Session.GetHabbo().LastMessageCount > 3)
            //     {
            //         StarBlueServer.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Usuário: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Vezes: " + Session.GetHabbo().LastMessageCount + "."));
            //          Session.GetHabbo().LastMessageCount = 0;
            //      }
            //  }

            StarBlueServer.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room.RoomData));
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") && StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out string word))
            {
                Session.GetHabbo().BannedPhraseCount++;

                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("Você mencionou uma palavra não apta para o hotel! Aviso " + Session.GetHabbo().BannedPhraseCount + "/10");

                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(StarBlueServer.GetUnixTimestamp()).ToLocalTime();

                    StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    StarBlueServer.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de divulgador:",
                    "<b><font color=\"#B40404\">Por favor, investigue bem antes de punir.</font></b><br><br>Palavra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Sequência:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));

                    if (Session.GetHabbo().BannedPhraseCount >= 10)
                    {
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", "O usuário " + Session.GetHabbo().Username + " foi banido automaticamente pelo sistema.", ""));

                        StarBlueServer.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Banido por fazer spam com a palavra (" + word + ")", (StarBlueServer.GetUnixTimestamp() + 78892200));
                        Session.Disconnect();
                        return;
                    }
                    return;
                }

                Session.SendMessage(new ChatComposer(User.VirtualId, "Mensagem inapropiada.", 0, Colour));
                return;
            }

            //if (Session.GetHabbo().IsBeingAsked == true && Message.ToLower().Contains("s"))
            //{
            //    Session.GetHabbo().SecureTradeEnabled = true;
            //    Session.GetHabbo().IsBeingAsked = false;
            //    Session.SendMessage(new WiredSmartAlertComposer("Acabas de activar el modo seguro de tradeo para dados."));
            //}
            //else if (Session.GetHabbo().IsBeingAsked == true && !Message.ToLower().Contains("s"))
            //{
            //    Session.GetHabbo().SecureTradeEnabled = false;
            //    Session.GetHabbo().IsBeingAsked = false;
            //    Session.SendMessage(new WiredSmartAlertComposer("Has dejado el tradeo en modo normal."));
            //}

            Session.GetHabbo().LastMessage = Message;

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);
            User.OnChat(User.LastBubble, Message, false);
        }
    }
}
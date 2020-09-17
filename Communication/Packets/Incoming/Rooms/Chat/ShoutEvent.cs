using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Chat.Styles;
using StarBlue.Utilities;
using System;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Chat
{
    public class ShoutEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
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

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

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
                Session.SendWhisper("O quarto está mudo, você não pode falar nele até que o proprietário ou alguém com permissão nele permita.");
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Opa, você se encontra silenciad@.");
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (User.IncrementAndCheckFlood(out int MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }

            //    if (Session.GetHabbo().LastMessage == Message)
            //    {
            //        Session.GetHabbo().LastMessageCount++;
            //        if (Session.GetHabbo().LastMessageCount > 3)
            //         {
            //              StarBlueServer.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Usuário: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Vezes: " + Session.GetHabbo().LastMessageCount + "."));
            //              Session.GetHabbo().LastMessageCount = 0;
            //          }
            //      }

            if (Message.Contains("&#1Âº;") || Message.Contains("&#1Âº") || Message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            Room.GetFilter().CheckMessage(Message);

            if (Message.StartsWith(":", StringComparison.CurrentCulture) && StarBlueServer.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
            {
                return;
            }

            StarBlueServer.GetGame().GetChatManager().GetLogs().StoreChatlog(new StarBlue.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room.RoomData));

            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out string word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("Você mencionou uma palavra inadequada para o  " + StarBlueServer.HotelName + "! Aviso " + Session.GetHabbo().BannedPhraseCount + "/10");

                    StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Mensagem: " + Message + " / Palavra: " + word.ToUpper() + " / Aviso: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    StarBlueServer.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de divulgador:",
                    "<b><font color=\"#B40404\">Lembre-se de investigar cuidadosamente antes de recorrer a uma sanção.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuário: " + Session.GetHabbo().Username + "</b><br><b>Sequência:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;

                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    StarBlueServer.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer spam con la frase (" + Message + ")", (StarBlueServer.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                Session.SendMessage(new ShoutComposer(User.VirtualId, "Mensagem inapropriada.", 0, Colour));
                return;
            }

            Session.GetHabbo().LastMessage = Message;
            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

            User.UnIdle();
            User.OnChat(User.LastBubble, Message, true);
        }
    }
}
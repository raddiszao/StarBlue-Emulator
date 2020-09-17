using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class SendMsgEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int userId = Packet.PopInt();
            if (userId == 0 || userId == Session.GetHabbo().Id)
            {
                return;
            }

            string message = Packet.PopString();
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendWhisper("Oops, você foi mutado por 15 minutos.");
                return;
            }

            if (message.Contains("&#1Âº;") || message.Contains("&#1Âº") || message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            //if (Session.GetHabbo().LastMessage == message)
            // {
            ////    Session.GetHabbo().LastMessageCount++;
            //   if (Session.GetHabbo().LastMessageCount > 3)
            // {
            //      StarBlueServer.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Usuário: " + Session.GetHabbo().Username + " / Mensagem: " + message + " / Vezes: " + Session.GetHabbo().LastMessageCount + "."));
            //       Session.GetHabbo().LastMessageCount = 0;
            //   }
            //   }

            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(message, out string word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 1;
                    Session.SendNotification("Acabou de mencionar uma palavra proibida no filtro " + StarBlueServer.HotelName + ", pode ser um erro, ou não, aviso " + Session.GetHabbo().BannedPhraseCount + " / 10");
                    StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + message + " / Palavra: " + word.ToUpper() + " / Aviso: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    StarBlueServer.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, investigue bem antes de punir.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/ 10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    StarBlueServer.GetGame().GetModerationManager().BanUser("STARBLUE", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Banido por divulgar com a frase (" + message + ")", (StarBlueServer.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }

            Session.GetHabbo().LastMessage = message;
            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);

        }
    }
}
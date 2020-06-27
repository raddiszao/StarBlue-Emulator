using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.SMS;
using static StarBlue.Core.Rank.RankManager;

namespace StarBlue.Communication.Packets.Incoming.SMS
{
    class SMSVerification : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            string Pin = Packet.PopString();
            if (Pin.Equals(""))
            {
                return;
            }

            if (Pin.ToLower() == Session.GetHabbo().PinClient.ToLower())
            {
                StarBlueServer.GetRankManager().TryGetValue(Session.GetHabbo().Rank, out RankData RankData);
                StarBlueServer.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble(RankData.Badge, RankData.Name + " " + Session.GetHabbo().Username + " verificou-se com sucesso!", ""));
                Session.SendMessage(new SMSErrorComposer());
                Session.SendMessage(new SMSVerifyComposer(-1, -1));
                Session.SendMessage(new GraphicAlertComposer("staffok"));
                Session.GetHabbo().StaffOk = true;
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                {
                    Session.SendMessage(new ModeratorInitComposer(
                    StarBlueServer.GetGame().GetModerationManager().UserMessagePresets,
                    StarBlueServer.GetGame().GetModerationManager().RoomMessagePresets,
                    StarBlueServer.GetGame().GetModerationManager().GetTickets));
                }
            }
            else
            {
                Session.SendMessage(new SMSVerifyComposer(1, 1));
                Session.SendMessage(new RoomCustomizedAlertComposer("PIN Incorreto, tente novamente!"));
                StarBlueServer.GetGame().GetClientManager().ManagerAlert(new SendHotelAlertLinkEventComposer("ATENÇÃO: O Staff " + Session.GetHabbo().Username + " falhou na autenticação do seu PIN."));
                LogCommand(Session.GetHabbo().Id, "PIN Inválido", Session.GetHabbo().MachineId, Session.GetHabbo().Username);
            }


        }
        public void LogCommand(int UserId, string Data, string MachineId, string Username)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", StarBlueServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }
    }
}

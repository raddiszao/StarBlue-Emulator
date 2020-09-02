using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.WebClient;
using static StarBlue.Core.Rank.RankManager;

namespace StarBlue.Communication.Packets.Incoming.WebSocket
{
    class PinVerifyEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            string Pin = Packet.PopString();
            if (Pin.Equals(""))
            {
                return;
            }

            if (Pin.ToLower() == Client.GetHabbo().PinClient.ToLower())
            {
                StarBlueServer.GetRankManager().TryGetValue(Client.GetHabbo().Rank, out RankData RankData);
                StarBlueServer.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble(RankData.Badge, RankData.Name + " " + Client.GetHabbo().Username + " verificou-se com sucesso!", ""));
                Client.SendWhisper("Você se verificou com sucesso.", 34);
                Client.GetHabbo().StaffOk = true;
                Client.GetHabbo().SendWebPacket(new PinVerifyComposer(""));
                if (Client.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                {
                    Client.SendMessage(new ModeratorInitComposer(
                    StarBlueServer.GetGame().GetModerationManager().UserMessagePresets,
                    StarBlueServer.GetGame().GetModerationManager().RoomMessagePresets,
                    StarBlueServer.GetGame().GetModerationManager().GetTickets));
                }
            }
            else
            {
                Client.GetHabbo().SendWebPacket(new PinVerifyComposer("error"));
                LogCommand(Client.GetHabbo().Id, "PIN Inválido", Client.GetHabbo().MachineId, Client.GetHabbo().Username);
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

using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Notifications;
using System.Data;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ViewClonesCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";

        public string Parameters => "[USUARIO]";

        public string Description => "Ver clones.";

        public void Execute(GameClients.GameClient session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                session.SendWhisper("Por favor, digite o nome do usuário para revisar.");
                return;
            }

            string str2;
            IQueryAdapter adapter;
            string username = Params[1];
            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            if (StarBlueServer.GetGame().GetClientManager().GetClientByUsername(username) != null)
            {
                str2 = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(username).GetConnection().GetIp();
                builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                using (adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.GetTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "Id : ", row["id"], " - Username: ", row["username"] }));
                    }
                }
                session.SendMessage(new MOTDNotificationComposer(builder.ToString()));
            }
            else
            {
                using (adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT ip_last FROM users WHERE username = @username");
                    adapter.AddParameter("username", username);
                    str2 = adapter.GetString();
                    builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.GetTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "Id : ", row["id"], " - Username: ", row["username"] }));
                    }
                }

                session.SendMessage(new MOTDNotificationComposer(builder.ToString()));
            }
            return;
        }
    }
}
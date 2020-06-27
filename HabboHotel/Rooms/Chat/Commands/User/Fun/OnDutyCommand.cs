using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Notifications;
using System.Data;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class OnDutyCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "Ve los usarios conectados ahora mismo."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            using (IQueryAdapter Adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                Adapter.SetQuery("SELECT username FROM users WHERE online = '1'");
                Adapter.RunQuery();

                DataTable Table = Adapter.GetTable();

                StringBuilder List = new StringBuilder("");
                int OnlineUsers = StarBlueServer.GetGame().GetClientManager().Count;
                List.AppendLine("Usarios conectados: " + OnlineUsers);
                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        List.AppendLine(Row["Username"].ToString());
                    }
                }
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }
        }
    }
}
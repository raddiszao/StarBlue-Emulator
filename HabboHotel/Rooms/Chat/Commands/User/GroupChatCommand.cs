using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class GroupChatCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Delete seu chat de grupo.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Ocorreu um erro, tente deletar", 34);
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Não tem permissão..", 34);
                return;
            }

            if (Room.RoomData.Group == null)
            {
                Session.SendWhisper("Esta sala não tem grupo, se acabou de criar um digite :unload", 34);
                return;
            }

            string mode = Params[1].ToLower();
            Groups.Group group = Room.RoomData.Group;

            if (mode == "encerrar")
            {
                if (group.HasChat == false)
                {
                    Session.SendWhisper("Este grupo não tem chat ainda.", 34);
                    return;
                }

                using (IQueryAdapter adap = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("UPDATE groups SET has_chat = '0' WHERE id = @id");
                    adap.AddParameter("id", group.Id);
                    adap.RunQuery();
                }
                group.HasChat = false;
                List<GameClient> GroupMembers = (from Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null select Client).ToList();
                foreach (GameClient Client in GroupMembers)
                {
                    if (Client != null)
                    {
                        continue;
                    }

                    Client.SendMessage(new FriendListUpdateComposer(group, -1));
                }
            }
            else
            {
                Session.SendNotification("Ocorreu um erro");
            }


        }
    }
}
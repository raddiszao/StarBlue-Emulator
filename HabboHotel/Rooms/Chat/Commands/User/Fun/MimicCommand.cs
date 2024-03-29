﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class MimicCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Copiar a roupa de outro usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja copiar a roupa.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente ou talvez o usuário não esteja online.", 34);
                return;
            }

            if (!TargetClient.GetHabbo().AllowMimic)
            {
                Session.SendWhisper(Params[1] + " desativou a opção de ter sua aparência copiada.", 34);
                return;
            }

            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente ou talvez o usuário não esteja online.", 34);
                return;
            }

            Session.GetHabbo().Gender = TargetUser.GetClient().GetHabbo().Gender;
            Session.GetHabbo().Look = TargetUser.GetClient().GetHabbo().Look;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `gender` = @gender, `look` = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gender", Session.GetHabbo().Gender);
                dbClient.AddParameter("look", Session.GetHabbo().Look);
                dbClient.AddParameter("id", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User != null)
            {
                Session.SendMessage(new AvatarAspectUpdateMessageComposer(Session.GetHabbo().Look, Session.GetHabbo().Gender));
                Session.SendMessage(new UserChangeComposer(User, true));
                Room.SendMessage(new UserChangeComposer(User, false));
            }

            foreach (HabboHotel.Users.Messenger.MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends())
            {
                if (buddy.client == null)
                {
                    continue;
                }

                Habbo _habbo = StarBlueServer.GetHabboById(buddy.UserId);
                if (_habbo != null && _habbo.GetMessenger() != null)
                {
                    if (_habbo.GetMessenger().GetFriendsIds().TryGetValue(Session.GetHabbo().Id, out MessengerBuddy value))
                    {
                        value.mLook = Session.GetHabbo().Look;
                        _habbo.GetMessenger().UpdateFriend(Session.GetHabbo().Id, Session, true);
                    }
                }
            }
        }
    }
}
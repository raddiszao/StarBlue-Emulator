using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class UpdateFigureDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            string Gender = Packet.PopString().ToUpper();
            string Look = StarBlueServer.GetFigureManager().ProcessFigure(Packet.PopString(), Gender);

            if (Look == Session.GetHabbo().Look)
            {
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastClothingUpdateTime).TotalSeconds <= 2.0)
            {
                Session.GetHabbo().ClothingUpdateWarnings += 1;
                if (Session.GetHabbo().ClothingUpdateWarnings >= 25)
                {
                    Session.GetHabbo().SessionClothingBlocked = true;
                }

                return;
            }

            if (Session.GetHabbo().SessionClothingBlocked)
            {
                return;
            }

            Session.GetHabbo().LastClothingUpdateTime = DateTime.Now;

            string[] AllowedGenders = { "M", "F" };
            if (!AllowedGenders.Contains(Gender))
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Lo sentimos, ha elegido un genero invalido"));
                return;
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_LOOK);

            Session.GetHabbo().Look = StarBlueServer.FilterFigure(Look);
            Session.GetHabbo().Gender = Gender.ToLower();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("look", Look);
                dbClient.AddParameter("gender", Gender);
                dbClient.RunQuery();
            }

            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AvatarLooks", 1);
            Session.SendMessage(new AvatarAspectUpdateMessageComposer(Look, Gender));
            if (Session.GetHabbo().Look.Contains("ha-1006"))
            {
                StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.WEAR_HAT);
            }

            if (Session.GetHabbo().InRoom)
            {
                RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (RoomUser != null)
                {
                    Session.SendMessage(new UserChangeComposer(RoomUser, true));
                    Session.GetHabbo().CurrentRoom.SendMessage(new UserChangeComposer(RoomUser, false));
                }
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
                        value.mLook = Look;
                        _habbo.GetMessenger().UpdateFriend(Session.GetHabbo().Id, Session, true);
                    }
                }
            }
        }
    }
}
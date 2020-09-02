using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.Utilities;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Avatar
{
    internal class ChangeMottoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendNotification("Opa, você está em silêncio - você não pode mudar a missão.");
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastMottoUpdateTime).TotalSeconds <= 2.0)
            {
                Session.GetHabbo().MottoUpdateWarnings += 1;
                if (Session.GetHabbo().MottoUpdateWarnings >= 25)
                {
                    Session.GetHabbo().SessionMottoBlocked = true;
                }

                return;
            }

            if (Session.GetHabbo().SessionMottoBlocked)
            {
                return;
            }

            Session.GetHabbo().LastMottoUpdateTime = DateTime.Now;

            string newMotto = StringCharFilter.Escape(Packet.PopString().Trim());

            if (newMotto.Length > 38)
            {
                newMotto = newMotto.Substring(0, 38);
            }

            if (newMotto == Session.GetHabbo().Motto)
            {
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override"))
            {
                newMotto = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(newMotto, out string word) ? "Spam" : newMotto;
            }

            Session.GetHabbo().Motto = newMotto;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `motto` = @motto WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("motto", newMotto);
                dbClient.RunQuery();
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;
                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null || User.GetClient() == null)
                {
                    return;
                }

                Room.SendMessage(new UserChangeComposer(User, false));
                foreach (MessengerBuddy Buddy in Session.GetHabbo().GetMessenger().GetFriends().ToList())
                {
                    if (Buddy == null)
                    {
                        continue;
                    }

                    Habbo _habbo = StarBlueServer.GetHabboById(Buddy.UserId);
                    if (_habbo != null && _habbo.GetMessenger() != null)
                    {
                        if (_habbo.GetMessenger().GetFriendsIds().TryGetValue(Session.GetHabbo().Id, out MessengerBuddy value))
                        {
                            value.mMotto = newMotto;
                            _habbo.GetMessenger().UpdateFriend(Session.GetHabbo().Id, Session, true);
                        }
                    }
                }
            }
        }
    }
}

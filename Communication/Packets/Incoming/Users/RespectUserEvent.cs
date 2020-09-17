
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;
using System.Threading.Tasks;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    class RespectUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom || Session.GetHabbo().GetStats().DailyRespectPoints <= 0)
                return;

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
                return;

            RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Packet.PopInt());
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo().Id == Session.GetHabbo().Id || User.IsBot)
                return;

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_RespectEarned", 1);

            Session.GetHabbo().GetStats().DailyRespectPoints -= 1;
            Session.GetHabbo().GetStats().RespectGiven += 1;
            User.GetClient().GetHabbo().GetStats().Respect += 1;
            if (!User.IsDancing && User.CurrentEffect == 0)
            {
                User.ApplyEffect(165);
                Task.Delay(5000).ContinueWith(A => User.ApplyEffect(0));

            }
            if (!User2.IsDancing && User2.CurrentEffect == 0)
            {

                User2.ApplyEffect(165);
                Task.Delay(5000).ContinueWith(A => User2.ApplyEffect(0));

            }

            if (Room.RoomData.RespectNotificationsEnabled && !Room.RoomData.RoomMuted)
                Room.SendMessage(new RespectNotificationComposer(User.GetClient().GetHabbo().Id, User.GetClient().GetHabbo().GetStats().Respect));

            Room.SendMessage(new ActionComposer(ThisUser.VirtualId, 7));
        }
    }
}
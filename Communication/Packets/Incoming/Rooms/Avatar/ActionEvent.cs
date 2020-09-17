using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Avatar
{
    public class ActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int Action = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (User.DanceId > 0)
            {
                User.DanceId = 0;
            }

            User.UnIdle();
            Room.SendMessage(new ActionComposer(User.VirtualId, Action));

            if (Action == 5) // idle
            {
                User.IsAsleep = true;
                Room.SendMessage(new SleepComposer(User, true));
                User.ApplyEffect(517);
                if (User.GetClient().GetHabbo().CurrentRoom.GetWired() != null)
                {
                    User.GetClient().GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserAfk, User.GetClient().GetHabbo());
                }
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE);
        }
    }
}
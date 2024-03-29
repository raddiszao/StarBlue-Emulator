﻿
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Avatar
{
    internal class DanceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.UnIdle();

            int DanceId = Packet.PopInt();
            if (DanceId < 0 || DanceId > 4)
            {
                DanceId = 0;
            }

            if (DanceId > 0 && User.CarryItemID > 0)
            {
                User.CarryItem(0);
            }

            if (Session.GetHabbo().Effects().CurrentEffect > 0)
            {
                Room.SendMessage(new AvatarEffectComposer(User.VirtualId, 0));
            }

            User.DanceId = DanceId;

            Room.SendMessage(new DanceComposer(User, DanceId));

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_DANCE);
            if (Room.GetRoomUserManager().GetRoomUsers().Count > 19)
            {
                StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.MASS_DANCE);
            }
        }
    }
}
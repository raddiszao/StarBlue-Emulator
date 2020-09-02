﻿
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int BuddyId = Packet.PopInt();
            if (BuddyId == 0 || BuddyId == Session.GetHabbo().Id)
            {
                return;
            }

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(BuddyId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            if (!Client.GetHabbo().InRoom)
            {
                Session.SendMessage(new FollowFriendFailedComposer(2));
                Session.GetHabbo().GetMessenger().UpdateFriend(Client.GetHabbo().Id, Client, true);
                return;
            }
            else if (Session.GetHabbo().CurrentRoom != null && Client.GetHabbo().CurrentRoom != null)
            {
                if (Session.GetHabbo().CurrentRoom.Id == Client.GetHabbo().CurrentRoom.Id)
                {
                    return;
                }
            }

            Session.SendMessage(new RoomForwardComposer(Client.GetHabbo().CurrentRoomId));
        }
    }
}

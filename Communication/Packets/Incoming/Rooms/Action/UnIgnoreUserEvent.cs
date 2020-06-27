using StarBlue.Communication.Packets.Outgoing.Rooms.Action;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    class UnIgnoreUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            String Username = Packet.PopString();

            Habbo User = StarBlueServer.GetHabboByUsername(Username);
            if (User == null || !Session.GetHabbo().MutedUsers.Contains(User.Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Remove(User.Id);
            Session.SendMessage(new IgnoreStatusComposer(3, Username));
        }
    }
}

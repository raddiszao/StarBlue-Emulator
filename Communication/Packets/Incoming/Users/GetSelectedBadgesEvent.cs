
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int UserId = Packet.PopInt();
            Habbo Habbo = StarBlueServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                return;
            }

            Session.GetHabbo().lastUserId = UserId;
            Session.SendMessage(new HabboUserBadgesComposer(Habbo, UserId));
        }
    }
}
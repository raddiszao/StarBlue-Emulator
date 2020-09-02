using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class ScrGetUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));

        }
    }
}


using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class GetUserTagsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);

            Session.SendMessage(new UserTagsComposer(UserId, TargetClient));
        }
    }
}

using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class UserTagsComposer : ServerPacket
    {
        public UserTagsComposer(int UserId, GameClient Session)
            : base(ServerPacketHeader.UserTagsMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteInteger(0);
        }
    }
}

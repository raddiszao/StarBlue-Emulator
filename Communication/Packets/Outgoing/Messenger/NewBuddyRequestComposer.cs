using StarBlue.HabboHotel.Cache;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
            base.WriteInteger(Habbo.Id);
            base.WriteString(Habbo.Username);
            base.WriteString(Habbo.Look);
        }
    }
}

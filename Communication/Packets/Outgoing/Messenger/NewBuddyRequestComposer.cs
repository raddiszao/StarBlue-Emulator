using StarBlue.HabboHotel.Cache;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class NewBuddyRequestComposer : MessageComposer
    {
        public UserCache UserCache { get; }

        public NewBuddyRequestComposer(UserCache Habbo)
            : base(Composers.NewBuddyRequestMessageComposer)
        {
            this.UserCache = Habbo;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserCache.Id);
            packet.WriteString(UserCache.Username);
            packet.WriteString(UserCache.Look);
        }
    }
}

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FindFriendsProcessResultComposer : MessageComposer
    {
        public bool Found { get; }

        public FindFriendsProcessResultComposer(bool Found)
            : base(Composers.FindFriendsProcessResultMessageComposer)
        {
            this.Found = Found;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(Found);
        }
    }
}
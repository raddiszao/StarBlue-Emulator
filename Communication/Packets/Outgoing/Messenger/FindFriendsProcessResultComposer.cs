namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FindFriendsProcessResultComposer : ServerPacket
    {
        public FindFriendsProcessResultComposer(bool Found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            base.WriteBoolean(Found);
        }
    }
}
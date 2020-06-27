namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int Score, bool CanVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
            base.WriteInteger(Score);
            base.WriteBoolean(CanVote);
        }
    }
}

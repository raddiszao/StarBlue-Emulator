namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class JoinQueueComposer : ServerPacket
    {
        public JoinQueueComposer(int GameId)
            : base(ServerPacketHeader.JoinQueueMessageComposer)
        {
            base.WriteInteger(GameId);
        }
    }
}

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class EventWinnerComposer : ServerPacket
    {
        public EventWinnerComposer(string roomName, string winner) : base(9)
        {
            base.WriteString(roomName);
            base.WriteString(winner);
        }
    }
}
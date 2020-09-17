using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class EventWinnerComposer : WebComposer
    {
        public EventWinnerComposer(string roomName, string winner) : base(9)
        {
            base.WriteString(roomName);
            base.WriteString(winner);
        }
    }
}
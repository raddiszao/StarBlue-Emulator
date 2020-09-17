using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class MentionUserComposer : WebComposer
    {
        public MentionUserComposer(string roomName, int roomId, string message, string userBy, string userByLook) : base(3)
        {
            base.WriteString(roomName);
            base.WriteInteger(roomId);
            base.WriteString(message);
            base.WriteString(userBy);
            base.WriteString(userByLook);
        }
    }
}
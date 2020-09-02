namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class UpdateMobiComposer : ServerPacket
    {
        public UpdateMobiComposer(string itemName, int rotation, string state) : base(8)
        {
            base.WriteString(itemName);
            base.WriteInteger(rotation);
            base.WriteString(state);
        }
    }
}
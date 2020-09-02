namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FurnitureAliasesComposer : ServerPacket
    {
        public FurnitureAliasesComposer()
            : base(ServerPacketHeader.FurnitureAliasesMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}

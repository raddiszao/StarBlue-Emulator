namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftingFoundComposer : ServerPacket
    {
        public CraftingFoundComposer(int count, bool found)
            : base(ServerPacketHeader.CraftingFoundMessageComposer) //resultado
        {
            base.WriteInteger(count); //hay mas?
            base.WriteBoolean(found); //encontrado
        }
    }
}
namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftingFoundComposer : MessageComposer
    {
        private int count { get; }
        private bool found { get; }

        public CraftingFoundComposer(int count, bool found)
            : base(Composers.CraftingFoundMessageComposer) //resultado
        {
            this.count = count;
            this.found = found;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(count); //hay mas?
            packet.WriteBoolean(found); //encontrado
        }
    }
}
namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FurnitureAliasesComposer : MessageComposer
    {
        public FurnitureAliasesComposer()
            : base(Composers.FurnitureAliasesMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}

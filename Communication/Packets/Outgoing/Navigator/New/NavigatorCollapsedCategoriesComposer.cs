namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorCollapsedCategoriesComposer : ServerPacket
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(ServerPacketHeader.NavigatorCollapsedCategoriesMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}

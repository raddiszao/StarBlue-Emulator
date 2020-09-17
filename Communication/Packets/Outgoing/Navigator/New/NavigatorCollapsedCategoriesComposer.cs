namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorCollapsedCategoriesComposer : MessageComposer
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(Composers.NavigatorCollapsedCategoriesMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}

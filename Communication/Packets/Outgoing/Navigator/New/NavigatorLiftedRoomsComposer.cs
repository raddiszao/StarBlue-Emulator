namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorLiftedRoomsComposer : MessageComposer
    {
        public NavigatorLiftedRoomsComposer()
            : base(Composers.NavigatorLiftedRoomsMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);//Count
            {
                packet.WriteInteger(1);//Flat Id
                packet.WriteInteger(0);//Unknown
                packet.WriteString("");//Image
                packet.WriteString("Caption");//Caption.
            }
        }
    }
}

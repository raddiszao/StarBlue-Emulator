namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class HideWiredConfigComposer : MessageComposer
    {
        public HideWiredConfigComposer()
            : base(Composers.HideWiredConfigMessageComposer)
        {
        }
        public override void Compose(Composer packet)
        {

        }
    }
}
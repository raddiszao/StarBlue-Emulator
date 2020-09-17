namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class ActionComposer : MessageComposer
    {
        private int VirtualId { get; }
        private int Action { get; }

        public ActionComposer(int VirtualId, int Action)
            : base(Composers.ActionMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Action = Action;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Action);
        }
    }
}
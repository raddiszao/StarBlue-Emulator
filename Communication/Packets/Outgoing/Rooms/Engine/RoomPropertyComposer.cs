namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomPropertyComposer : MessageComposer
    {
        public string Name { get; }
        public string Val { get; }

        public RoomPropertyComposer(string name, string val)
            : base(Composers.RoomPropertyMessageComposer)
        {
            this.Name = name;
            this.Val = val;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Name);
            packet.WriteString(Val);
        }
    }
}

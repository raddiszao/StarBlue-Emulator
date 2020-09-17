namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class ClubGiftRecievedComposer : MessageComposer
    {
        public ClubGiftRecievedComposer() : base(Composers.ClubGiftRecievedComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("PENE");
            packet.WriteInteger(1);
            packet.WriteString("b"); // tipo de furni
            packet.WriteString("ADMIN"); // nombre
        }
    }
}
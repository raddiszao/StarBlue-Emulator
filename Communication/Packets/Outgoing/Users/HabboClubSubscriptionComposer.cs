namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class HabboClubSubscriptionComposer : MessageComposer
    {
        public HabboClubSubscriptionComposer() : base(Composers.HabboClubSubscriptionComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("club_habbo");
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(2);
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
        }
    }
}
namespace StarBlue.Communication.Packets.Outgoing.Users
{
    class HabboClubSubscriptionComposer : ServerPacket
    {
        public HabboClubSubscriptionComposer() : base(ServerPacketHeader.HabboClubSubscriptionComposer)
        {
            base.WriteString("club_habbo");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(2);
            base.WriteBoolean(false);
            base.WriteBoolean(false);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
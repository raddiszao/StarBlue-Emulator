namespace StarBlue.Communication.Packets.Outgoing.BuildersClub
{
    internal class BuildersClubMembershipComposer : MessageComposer
    {
        public BuildersClubMembershipComposer()
            : base(Composers.BuildersClubMembershipMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(int.MaxValue);
            packet.WriteInteger(100);
            packet.WriteInteger(0);
            packet.WriteInteger(int.MaxValue);
        }
    }
}

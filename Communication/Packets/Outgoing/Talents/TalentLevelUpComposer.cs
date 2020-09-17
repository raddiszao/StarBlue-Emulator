using StarBlue.HabboHotel.Achievements;

namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class TalentLevelUpComposer : MessageComposer
    {
        private Talent talent { get; }

        public TalentLevelUpComposer(Talent talent)
            : base(Composers.TalentLevelUpMessageComposer)
        {
            this.talent = talent;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(talent.Type);
            packet.WriteInteger(talent.Level);
            packet.WriteInteger(0);

            if (talent.Type == "citizenship" && talent.Level == 4)
            {
                packet.WriteInteger(2);
                packet.WriteString("HABBO_CLUB_VIP_7_DAYS");
                packet.WriteInteger(7);
                packet.WriteString(talent.Prize);
                packet.WriteInteger(0);
            }
            else
            {
                packet.WriteInteger(1);
                packet.WriteString(talent.Prize);
                packet.WriteInteger(0);
            }
        }
    }
}

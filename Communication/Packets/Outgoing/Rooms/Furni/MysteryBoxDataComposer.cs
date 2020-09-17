using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class MysteryBoxDataComposer : MessageComposer
    {
        private Habbo Habbo { get; }

        public MysteryBoxDataComposer(Habbo Habbo)
            : base(Composers.MysteryBoxDataComposer)
        {
            this.Habbo = Habbo;
        }

        public override void Compose(Composer packet)
        {
            foreach (string box in Habbo.MysticBoxes.ToArray())
            {
                packet.WriteString(box);
            }
            foreach (string key in Habbo.MysticKeys.ToArray())
            {
                packet.WriteString(key);
            }
        }
    }
}

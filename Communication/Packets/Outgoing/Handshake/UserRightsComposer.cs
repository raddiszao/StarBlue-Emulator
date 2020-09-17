using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    public class UserRightsComposer : MessageComposer
    {
        private Habbo habbo { get; }

        public UserRightsComposer(Habbo habbo)
            : base(Composers.UserRightsMessageComposer)
        {
            this.habbo = habbo;
        }

        public override void Compose(Composer packet)
        {
            if (habbo.GetClubManager().HasSubscription("habbo_vip"))
            {
                packet.WriteInteger(2);
            }
            else if (habbo.GetClubManager().HasSubscription("habbo_club"))
            {
                packet.WriteInteger(1);
            }
            else
            {
                packet.WriteInteger(0);
            }

            packet.WriteInteger(habbo.Rank);
            packet.WriteBoolean(habbo.Rank > 3);//Is an ambassador
        }
    }
}
using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class ScrSendUserInfoComposer : MessageComposer
    {
        private Habbo habbo { get; }

        public ScrSendUserInfoComposer(Habbo habbo)
            : base(Composers.ScrSendUserInfoMessageComposer)
        {
            this.habbo = habbo;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("habbo_club");

            if (habbo.GetClubManager().HasSubscription("habbo_vip"))
            {

                double Expire = habbo.GetClubManager().GetSubscription("habbo_vip").ExpireTime;
                double TimeLeft = Expire - StarBlueServer.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                int MonthsLeft = TotalDaysLeft / 31;

                if (MonthsLeft >= 1)
                {
                    MonthsLeft--;
                }
                packet.WriteInteger(TotalDaysLeft - (MonthsLeft * 31));
                packet.WriteInteger(2); // ??
                packet.WriteInteger(MonthsLeft);
                packet.WriteInteger(1); // type
                packet.WriteBoolean(true);
                packet.WriteBoolean(true);
                packet.WriteInteger(0);
                packet.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on hc
                packet.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on vip
            }
            else
            {
                packet.WriteInteger(0);
                packet.WriteInteger(0); // ??
                packet.WriteInteger(0);
                packet.WriteInteger(0); // type
                packet.WriteBoolean(false);
                packet.WriteBoolean(true);
                packet.WriteInteger(0);
                packet.WriteInteger(100); // days i have on hc
                packet.WriteInteger(100); // days i have on vip
            }
        }
    }
}

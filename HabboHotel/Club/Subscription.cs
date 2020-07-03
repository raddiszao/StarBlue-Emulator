using StarBlue.Core;

namespace StarBlue.HabboHotel.Club
{
    public class Subscription
    {
        private readonly string Caption;
        private int TimeExpire;

        internal string SubscriptionId => Caption;

        internal int ExpireTime => TimeExpire;

        internal Subscription(string Caption, int TimeExpire)
        {
            this.Caption = Caption;
            this.TimeExpire = TimeExpire;
        }

        internal bool IsValid()
        {
            return TimeExpire > StarBlueServer.GetUnixTimestamp();
        }

        internal void SetEndTime(int time)
        {
            TimeExpire = time;
        }

        internal void ExtendSubscription(int Time)
        {
            try
            {
                TimeExpire = (TimeExpire + Time);
            }
            catch
            {
                Logging.LogException("T: " + TimeExpire + "." + Time);
            }
        }
    }
}

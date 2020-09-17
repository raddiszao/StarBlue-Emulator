using System;

namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class LimitedCountdownExtendedComposer : MessageComposer
    {
        public LimitedCountdownExtendedComposer()
            : base(Composers.LimitedCountdownExtendedComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            string date = "20/01/2018 21:00:00.0";
            DateTime.TryParse(date, out DateTime fechilla);
            TimeSpan diff = fechilla - DateTime.Now;

            packet.WriteInteger(Convert.ToInt32(diff.TotalSeconds)); // Total Seconds
            packet.WriteInteger(-1); // PageID
            packet.WriteInteger(0); // ItemID
            packet.WriteString("throne"); // Productdata IMG
        }
    }
}
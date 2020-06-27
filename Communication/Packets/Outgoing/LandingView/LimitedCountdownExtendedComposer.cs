using System;

namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    class LimitedCountdownExtendedComposer : ServerPacket
    {
        public LimitedCountdownExtendedComposer()
            : base(ServerPacketHeader.LimitedCountdownExtendedComposer)
        {
            string date = "20/01/2018 21:00:00.0";
            DateTime.TryParse(date, out DateTime fechilla);
            TimeSpan diff = fechilla - DateTime.Now;

            base.WriteInteger(Convert.ToInt32(diff.TotalSeconds)); // Total Seconds
            base.WriteInteger(-1); // PageID
            base.WriteInteger(0); // ItemID
            base.WriteString("throne"); // Productdata IMG
        }
    }
}
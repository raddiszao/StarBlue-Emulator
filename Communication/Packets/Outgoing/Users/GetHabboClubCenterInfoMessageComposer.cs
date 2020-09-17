using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class GetHabboClubCenterInfoMessageComposer : MessageComposer
    {
        private GameClient Session { get; }

        public GetHabboClubCenterInfoMessageComposer(GameClient Session) : base(Composers.HabboClubCenterInfoMessageComposer)
        {
            this.Session = Session;
        }

        public override void Compose(Composer packet)
        {
            DateTime valecrack = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            valecrack = valecrack.AddSeconds(Session.GetHabbo().AccountCreated).ToLocalTime();

            string time = valecrack.ToString();

            packet.WriteInteger(2005);//streakduration in days 
            packet.WriteString(time);//joindate 
            packet.WriteInteger(0); packet.WriteInteger(0);//this should be a double 
            packet.WriteInteger(0);//unused 
            packet.WriteInteger(0);//unused 
            packet.WriteInteger(10);//spentcredits 
            packet.WriteInteger(20);//streakbonus 
            packet.WriteInteger(10);//spentcredits 
            packet.WriteInteger(50);//next pay in minutes
        }
    }
}
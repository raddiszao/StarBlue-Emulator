using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Purse
{
    class GetHabboClubCenterInfoMessageComposer : ServerPacket
    {
        public GetHabboClubCenterInfoMessageComposer(GameClient Session) : base(ServerPacketHeader.HabboClubCenterInfoMessageComposer)
        {

            DateTime valecrack = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            valecrack = valecrack.AddSeconds(Session.GetHabbo().AccountCreated).ToLocalTime();

            string time = valecrack.ToString();

            base.WriteInteger(2005);//streakduration in days 
            base.WriteString(time);//joindate 
            base.WriteInteger(0); base.WriteInteger(0);//this should be a double 
            base.WriteInteger(0);//unused 
            base.WriteInteger(0);//unused 
            base.WriteInteger(10);//spentcredits 
            base.WriteInteger(20);//streakbonus 
            base.WriteInteger(10);//spentcredits 
            base.WriteInteger(50);//next pay in minutes
        }
    }
}
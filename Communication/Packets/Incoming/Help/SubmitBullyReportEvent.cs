using StarBlue.Communication.Packets.Outgoing.Help;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.Communication.Packets.Incoming.Help
{
    class SubmitBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //0 = sent, 1 = blocked, 2 = no chat, 3 = already reported.
            if (Session == null)
            {
                return;
            }

            int UserId = Packet.PopInt();
            if (UserId == Session.GetHabbo().Id)//Hax
            {
                return;
            }

            if (Session.GetHabbo().AdvertisingReportedBlocked)
            {
                Session.SendMessage(new SubmitBullyReportComposer(1));//This user is blocked from reporting.
                return;
            }

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(UserId));
            if (Client == null)
            {
                Session.SendMessage(new SubmitBullyReportComposer(0));//Just say it's sent, the user isn't found.
                return;
            }

            if (Session.GetHabbo().LastAdvertiseReport > StarBlueServer.GetUnixTimestamp())
            {
                Session.SendNotification("Você pode realizar reports de 5 em 5 minutos.");
                return;
            }

            if (Client.GetHabbo().GetPermissions().HasRight("mod_tool"))//Reporting staff, nope!
            {
                Session.SendNotification("Você não pode reportar um staff .");
                return;
            }

            //This user hasn't even said a word, nope!
            if (!Client.GetHabbo().HasSpoken)
            {
                Session.SendMessage(new SubmitBullyReportComposer(2));
                return;
            }

            //Already reported, nope.
            if (Client.GetHabbo().AdvertisingReported && Session.GetHabbo().Rank < 2)
            {
                Session.SendMessage(new SubmitBullyReportComposer(3));
                return;
            }

            if (Session.GetHabbo().Rank <= 1)
            {
                Session.GetHabbo().LastAdvertiseReport = StarBlueServer.GetUnixTimestamp() + 300;
            }
            else
            {
                Session.GetHabbo().LastAdvertiseReport = StarBlueServer.GetUnixTimestamp();
            }

            Client.GetHabbo().AdvertisingReported = true;
            Session.SendMessage(new SubmitBullyReportComposer(0));
            //StarBlueServer.GetGame().GetClientManager().ModAlert("New advertising report! " + Client.GetHabbo().Username + " has been reported for advertising by " + Session.GetHabbo().Username +".");
            StarBlueServer.GetGame().GetClientManager().DoAdvertisingReport(Session, Client);
            return;
        }
    }
}
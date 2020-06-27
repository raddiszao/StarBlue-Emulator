using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;
using System;
using System.Text;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class HelperSessionChatSendMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Element = HelperToolsManager.GetElement(Session);
            var message = Packet.PopString();
            if (Element.OtherElement != null)
            {
                Session.SendMessage(new HelperSessionSendChatComposer(Session.GetHabbo().Id, message));
                Element.OtherElement.Session.SendMessage(new HelperSessionSendChatComposer(Session.GetHabbo().Id, message));
                LogHelper(Session.GetHabbo().Id, Element.OtherElement.Session.GetHabbo().Id, message);
            }
            else
            {
                Session.SendMessage(new CallForHelperErrorComposer(0));
            }
        }

        public void LogHelper(int From_Id, int ToId, string Message)
        {
            DateTime Now = DateTime.Now;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO chatlogs_helper VALUES (NULL, " + From_Id + ", " + ToId + ", @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Encoding.UTF8.GetString(Encoding.Default.GetBytes(Message)));
                dbClient.RunQuery();
            }
        }
    }
}

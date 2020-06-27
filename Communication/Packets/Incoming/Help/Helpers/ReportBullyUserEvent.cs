using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class ReportBullyUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            //int userId = Packet.PopInt();
            //int roomId = Packet.PopInt();
            //string message = Packet.PopString();

            //Session.SendWhisper(message);

            //var call = HelperToolsManager.AddCall(Session, "", 3);
            //var helpers = HelperToolsManager.GetHelpersToCase(call).FirstOrDefault();

            //if (helpers != null)
            //{
            //    HelperToolsManager.InviteGuardianCall(helpers, call);
            //}

            //var response = new ServerPacket(ServerPacketHeader.GuideSessionErrorMessageComposer);
            //response.WriteInteger(0);
            //Session.SendMessage(response);
            //return;
        }
    }
}

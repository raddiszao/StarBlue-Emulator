using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class AcceptJoinJudgeChatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            //bool request = Packet.PopBoolean();

            //switch(request)
            //{
            //    case true:
            //        var response = new ServerPacket(ServerPacketHeader.GuardianSendChatCaseMessageComposer);
            //        response.WriteInteger(60);
            //        response.WriteString("");
            //        Session.SendMessage(response);
            //        break;
            //    case false:

            //        break;
            //}
        }
    }
}

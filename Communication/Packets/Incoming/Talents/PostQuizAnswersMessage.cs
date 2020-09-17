using StarBlue.Communication.Packets.Outgoing.Talents;

namespace StarBlue.Communication.Packets.Incoming.Talents
{
    internal class PostQuizAnswersMessage : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            string HabboType = Packet.PopString();
            if (HabboType != "HabboWay1")
            {
                return;
            }

            int HabboQuestions = Packet.PopInt();
            Session.SendMessage(new PostQuizAnswersMessageComposer(Session.GetHabbo(), HabboType, Packet, HabboQuestions));
        }
    }
}

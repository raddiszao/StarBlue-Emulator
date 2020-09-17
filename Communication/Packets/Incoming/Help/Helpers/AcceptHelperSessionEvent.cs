using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class AcceptHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            bool Accepted = Packet.PopBoolean();
            HabboHelper Helper = HelperToolsManager.GetHelper(Session);

            if (Helper == null)
            {
                Session.SendMessage(new Outgoing.Help.Helpers.CloseHelperSessionComposer());
                return;
            }

            if (Accepted)
            {
                Helper.Accept();
            }
            else
            {
                Helper.Decline();
            }
        }
    }
}

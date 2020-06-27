using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class CancelCallForHelperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var call = HelperToolsManager.GetCall(Session);
            HelperToolsManager.RemoveCall(call);
            Session.SendMessage(new CloseHelperSessionComposer());
            if (call.Helper != null)
            {
                call.Helper.CancelCall();
            }

            Session.SendMessage(new CloseHelperSessionComposer());
        }
    }
}

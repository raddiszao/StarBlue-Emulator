using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class CallForHelperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var category = Packet.PopInt();
            var message = Packet.PopString();

            var helper = HelperToolsManager.GetHelper(Session);
            if (helper != null)
            {
                Session.SendNotification("Pedido de ajuda enviado com sucesso.");
                Session.SendMessage(new CloseHelperSessionComposer());
                return;
            }

            var call = HelperToolsManager.AddCall(Session, message, category);
            var helpers = HelperToolsManager.GetHelpersToCase(call).FirstOrDefault();

            if (helpers != null)
            {
                HelperToolsManager.InvinteHelpCall(helpers, call);
                Session.SendMessage(new CallForHelperWindowComposer(false, call));
                return;
            }

            Session.SendMessage(new CallForHelperErrorComposer(1));

        }
    }
}

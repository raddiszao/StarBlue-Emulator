using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class CallForHelperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int category = Packet.PopInt();
            string message = Packet.PopString();

            HabboHotel.Helpers.HabboHelper helper = HelperToolsManager.GetHelper(Session);
            if (helper != null)
            {
                Session.SendNotification("Pedido de ajuda enviado com sucesso.");
                Session.SendMessage(new CloseHelperSessionComposer());
                return;
            }

            HelperCase call = HelperToolsManager.AddCall(Session, message, category);
            HabboHotel.Helpers.HabboHelper helpers = HelperToolsManager.GetHelpersToCase(call).FirstOrDefault();

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

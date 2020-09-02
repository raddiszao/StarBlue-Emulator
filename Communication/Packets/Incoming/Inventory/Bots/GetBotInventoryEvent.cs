using StarBlue.Communication.Packets.Outgoing.Inventory.Bots;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Bots
{
    internal class GetBotInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            ICollection<Bot> Bots = Session.GetHabbo().GetInventoryComponent().GetBots();
            Session.SendMessage(new BotInventoryComposer(Bots));
        }
    }
}

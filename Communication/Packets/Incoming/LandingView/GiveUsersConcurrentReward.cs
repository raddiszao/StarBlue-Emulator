using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using System;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    class GiveConcurrentUsersReward : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetStats().PurchaseUsersConcurrent)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Você recebeu um prêmio."));
            }

            string badge = StarBlueServer.GetSettingsManager().TryGetValue("usersconcurrent_badge");
            int pixeles = int.Parse(StarBlueServer.GetSettingsManager().TryGetValue("usersconcurrent_pixeles"));

            Session.GetHabbo().GOTWPoints = Session.GetHabbo().GOTWPoints + pixeles;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, pixeles, 103));
            Session.GetHabbo().GetBadgeComponent().GiveBadge(badge, true, Session);
            Session.SendMessage(new RoomCustomizedAlertComposer("Você recebeu um emblema e " + pixeles + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + "."));
            Session.GetHabbo().GetStats().PurchaseUsersConcurrent = true;
        }
    }
}

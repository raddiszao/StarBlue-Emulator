using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog.Vouchers;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Data;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            if (!StarBlueServer.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher Voucher))
            {
                Session.SendMessage(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
            {
                Session.SendNotification("Este código de cupom foi usado quantas vezes for permitido!");
                return;
            }

            DataRow GetRow = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("Voucher", VoucherCode);
                GetRow = dbClient.GetRow();
            }

            if (GetRow != null)
            {
                Session.SendNotification("Você já usou este código Voucher!!");
                return;
            }
            else
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES ('" + Session.GetHabbo().Id + "', @Voucher)");
                    dbClient.AddParameter("Voucher", VoucherCode);
                    dbClient.RunQuery();
                }
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetHabbo().Credits += Voucher.Value;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Você acabou de receber um prêmio de voucher pelo valor de " + Voucher.Value + " créditos. ¡Úsalos con cabeza, " + Session.GetHabbo().Username + ".", ""));
            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetHabbo().Duckets += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Voucher.Value));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Você acabou de receber um prêmio de voucher pelo valor de " + Voucher.Value + " duckets. ¡Úsalos con cabeza, " + Session.GetHabbo().Username + ".", ""));
            }
            else if (Voucher.Type == VoucherType.DIAMOND)
            {
                Session.GetHabbo().Diamonds += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Voucher.Value, 5));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Você acabou de receber um prêmio de voucher pelo valor de " + Voucher.Value + " diamantes.", ""));
            }
            else if (Voucher.Type == VoucherType.HONOR)
            {
                Session.GetHabbo().GOTWPoints += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, Voucher.Value, 103));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Você acabou de receber um prêmio de voucher pelo valor de " + Voucher.Value + " " + StarBlueServer.GetSettingsManager().TryGetValue("seasonal.currency.name") + ".", ""));
            }
            else if (Voucher.Type == VoucherType.ITEM)
            {

                if (!StarBlueServer.GetGame().GetItemManager().GetItem((Voucher.Value), out ItemData Item))
                {
                    // No existe este ItemId.
                    return;
                }

                Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Session.SendMessage(new FurniListUpdateComposer());
                    Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un objeto raro desde un voucher. ¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer!", ""));
                }

                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
            }

            Session.SendMessage(new VoucherRedeemOkComposer());
        }
    }
}
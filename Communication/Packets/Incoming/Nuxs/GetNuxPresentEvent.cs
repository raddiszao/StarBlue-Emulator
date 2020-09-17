using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.Utilities;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Nux
{
    internal class GetNuxPresentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int Data1 = Packet.PopInt(); // ELEMENTO 1
            int Data2 = Packet.PopInt(); // ELEMENTO 2
            int Data3 = Packet.PopInt(); // ELEMENTO 2
            int Data4 = Packet.RemainingLength() >= 4 ? Packet.PopInt() : RandomNumber.GenerateRandom(0, 2); // SELECTOR
            string RewardName = "";

            switch (Data4)
            {
                case 0:
                    int RewardDiamonds = RandomNumber.GenerateRandom(0, 5);
                    Session.GetHabbo().Diamonds += RewardDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, RewardDiamonds, 5));
                    break;
                case 1:
                    int RewardDuckets = RandomNumber.GenerateRandom(25, 50);
                    Session.GetHabbo().Duckets += RewardDuckets;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, RewardDuckets));
                    Session.SendMessage(RoomNotificationComposer.SendBubble("duckets", "Você recebeu " + RewardDuckets + " Duckets!", ""));
                    break;
                case 2:
                    int RewardItem = RandomNumber.GenerateRandom(1, 8);
                    int RewardItemId = 0;

                    switch (RewardItem)
                    {
                        case 1:
                            RewardItemId = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                            RewardName = "Elefante Azul Colorido";
                            break;
                        case 4:
                            RewardItemId = 1587; // Lámpara Calippo - ads_calip_lava
                            RewardName = "Lampada de Lava Calippo";
                            break;
                        case 6:
                            RewardItemId = 385; // Toldo Amarillo - marquee*4
                            RewardName = "Toldo Amarelo";
                            break;
                        case 8:
                            RewardItemId = 9506; // Parasol Azul - rare_colourable_parasol*1
                            RewardName = "Parasol Azul";
                            break;
                        default:
                            Session.SendWhisper("Não foi dessa vez, tente na próxima!", 34);
                            break;
                    }

                    if (RewardItemId == 0)
                        return;

                    ItemData Item = null;
                    if (!StarBlueServer.GetGame().GetItemManager().GetItem(RewardItemId, out Item))
                    { return; }

                    Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                    if (GiveItem != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                        Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                        Session.SendMessage(new FurniListUpdateComposer());
                        Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acaba de receber um " + RewardName + ".\n\n" + Session.GetHabbo().Username + ", tem algo novo no seu inventário!", ""));
                    }

                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    break;
            }
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AnimationRanking", 1);
        }
    }
}
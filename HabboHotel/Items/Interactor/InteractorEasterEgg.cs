using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;

namespace StarBlue.HabboHotel.Items.Interactor
{
    internal class InteractorEasterEgg : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.FoundBy = string.Empty;
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if ((!string.IsNullOrEmpty(Item.FoundBy)))
            {
                if (Session.GetHabbo().Username != Item.FoundBy)
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("easteregg", "Este ovo foi encontrado por " + Item.FoundBy + "!", ""));
                    return;
                }
            }
            else

            if (Session == null || Session.GetHabbo() == null || Item == null)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
            {
                return;
            }

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
            {
                Item.FoundBy = Session.GetHabbo().Username;
            }

            int tick = int.Parse(Item.ExtraData);

            if (tick < 19)
            {
                if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
                {
                    tick++;
                    Item.ExtraData = tick.ToString();
                    Item.UpdateState(true, true);
                    int X = Item.GetX, Y = Item.GetY, Rot = Item.Rotation;
                    double Z = Item.GetZ;
                    if (tick == 19)
                    {
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                            dbClient.RunFastQuery("DELETE FROM items WHERE id = " + Item.Id);
                        }

                        // Empezamos a generar el tipo de premio según lotería.
                        int RewardType = RandomNumber.GenerateRandom(1, 20);
                        switch (RewardType)
                        {
                            case 1:
                                int RewardDiamonds = RandomNumber.GenerateRandom(1, 10);
                                Session.GetHabbo().Diamonds += RewardDiamonds;
                                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, RewardDiamonds, 5));
                                Session.SendWhisper("Acabas de ganhar " + RewardDiamonds + " diamantes com este ovo de páscoa, que sorte!.", 34);
                                break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 8:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                                Session.GetHabbo().GOTWPoints += 1;
                                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 1, 103));
                                Session.SendMessage(RoomNotificationComposer.SendBubble("easteregg", "Acabas de conseguir un Huevo de Pascua, colecciona varios para canjearlos por premios.", ""));
                                break;
                            case 7:
                                ItemData RewardItemPrize = null;
                                if (!StarBlueServer.GetGame().GetItemManager().GetItem(9780, out RewardItemPrize))
                                { return; }

                                Item GiveItem = ItemFactory.CreateSingleItemNullable(RewardItemPrize, Session.GetHabbo(), "", "");
                                if (GiveItem != null)
                                {
                                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                                    Session.SendMessage(new FurniListUpdateComposer());
                                    Session.SendMessage(RoomNotificationComposer.SendBubble("easteregg", "Acabas de recibir un Huevo de Pascua raro.\n\n¡Corre, " + Session.GetHabbo().Username + ", haz click aquí y revisa tu inventario!", "inventory/open"));
                                }

                                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                                break;
                        }

                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_EggCracker", 1);

                    }
                }

            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}

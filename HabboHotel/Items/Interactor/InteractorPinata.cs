using StarBlue.HabboHotel.Rooms;

namespace StarBlue.HabboHotel.Items.Interactor
{
    class InteractorPinata : IFurniInteractor
    {
        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
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

            if (Item.ExtraData == "1")
            {
                return;
            }

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 2)
            {
                return;
            }

            StarBlueServer.GetGame().GetPinataManager().ReceiveCrackableReward(Actor, Room, Item);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_PinataWhacker", 1);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_PinataBreaker", 1);

        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}

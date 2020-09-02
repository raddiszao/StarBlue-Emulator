﻿using StarBlue.HabboHotel.Rooms;

namespace StarBlue.HabboHotel.Items.Interactor
{
    internal class InteractorViking : IFurniInteractor
    {
        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateState(true, true);
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

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 2)
            {
                return;
            }

            if (Actor.CurrentEffect == 5)
            {
                int count = int.Parse(Item.ExtraData);
                if (count < 5)
                {
                    count++;
                    Item.ExtraData = count + "";
                    Item.UpdateState(true, true);
                }
                if (count == 5)
                {
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_ViciousViking", 1);
                }
                return;
            }

        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}

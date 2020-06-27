using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using System;

namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorBanzaiScoreCounter : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            if (Item.team == TEAM.NONE)
            {
                return;
            }

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[Convert.ToInt32(Item.team)].ToString();
            Item.UpdateState(false, true);
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (HasRights)
            {
                Item.GetRoom().GetGameManager().Points[Convert.ToInt32(Item.team)] = 0;

                Item.ExtraData = "0";
                Item.UpdateState();
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}
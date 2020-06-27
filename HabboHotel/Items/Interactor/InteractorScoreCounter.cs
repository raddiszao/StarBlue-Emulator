using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using System;

namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorScoreCounter : IFurniInteractor
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
            if (!HasRights)
            {
                return;
            }


            if (!int.TryParse(Item.ExtraData, out int OldValue))
            {
            }

            if (Request == 1)
            {
                OldValue++;
            }
            else if (Request == 2)
            {
                OldValue--;
            }
            else if (Request == 3)
            {
                OldValue = 0;
            }

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState(false, true);
        }

        public void OnWiredTrigger(Item Item)
        {

            if (!int.TryParse(Item.ExtraData, out int OldValue))
            {
            }

            OldValue++;

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState(false, true);
        }
    }
}
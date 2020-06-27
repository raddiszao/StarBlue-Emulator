using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorScoreboard : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
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
                if (Item.pendingReset && OldValue > 0)
                {
                    OldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    OldValue = OldValue + 60;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 2)
            {
                Item.UpdateNeeded = !Item.UpdateNeeded;
                Item.pendingReset = true;
            }


            Item.ExtraData = OldValue.ToString();
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {

            if (!int.TryParse(Item.ExtraData, out int OldValue))
            {
            }

            OldValue = OldValue + 60;
            Item.UpdateNeeded = false;

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState();
        }
    }
}
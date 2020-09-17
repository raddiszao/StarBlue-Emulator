using StarBlue.Communication.Packets.Outgoing;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorRentableSpace : IFurniInteractor
    {
        public void SerializeExtradata(Composer Message, Item Item)
        {


        }

        public void OnPlace(GameClient Session, Item Item)
        {
            int Tick = 1000;

            if (Tick > 0)
            {
                Item.ExtraData = Tick.ToString();
                Item.UpdateState();
            }
            else
            {
                Tick--;
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            int Modes = Item.GetBaseItem().Modes - 1;

            if (Session == null || !HasRights || Modes <= 0)
            {
                return;
            }


            int NewMode = 0;

            if (!int.TryParse(Item.ExtraData, out int CurrentMode))
            {
            }

            if (CurrentMode <= 0)
            {
                NewMode = 1;
            }
            else if (CurrentMode >= Modes)
            {
                NewMode = 0;
            }
            else
            {
                NewMode = CurrentMode + 1;
            }
            Session.SendWhisper(NewMode.ToString());

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            return;
        }

        public void OnCycle(Item Item)
        {
        }

    }
}

using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.HabboHotel.Items.Interactor
{
    internal class InteractorMuteSignal : IFurniInteractor
    {
        private const int Modes = 1;

        public void OnPlace(GameClient Session, Item Item) { }
        public void OnRemove(GameClient Session, Item Item) { }
        public void OnWiredTrigger(Item Item) { }
        public void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }
            int currentMode = 0;
            int newMode = 0;

            try
            {
                currentMode = int.Parse(Item.ExtraData);
            }
            catch
            {

            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= Modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            //1 = muted, 0 = no mute
            Room UserRoom = Item.GetRoom();
            switch (newMode)
            {
                case 0:
                    UserRoom.muteSignalEnabled = false;
                    break;

                case 1:
                    UserRoom.muteSignalEnabled = true;
                    break;
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
        }
    }
}

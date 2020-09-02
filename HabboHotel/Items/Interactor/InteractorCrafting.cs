using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Items.Interactor
{
    internal class InteractorCrafting : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Session.SendMessage(new MassEventComposer("inventory/open"));
            Session.SendMessage(new CraftableProductsComposer(Item));
            Session.GetHabbo().LastCraftingMachine = Item.GetBaseItem().Id;
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}
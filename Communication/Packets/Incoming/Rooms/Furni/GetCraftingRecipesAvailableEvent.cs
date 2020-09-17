using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    internal class GetCraftingRecipesAvailableEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int craftingTable = Packet.PopInt();
            List<Item> items = new List<Item>();

            int count = Packet.PopInt();
            for (int i = 1; i <= count; i++)
            {
                int id = Packet.PopInt();

                Item item = Session.GetHabbo().GetInventoryComponent().GetItem(id);
                if (item == null || items.Contains(item))
                {
                    return;
                }

                items.Add(item);
            }

            CraftingRecipe craftingRecipe = null;
            foreach (KeyValuePair<string, CraftingRecipe> recipe in StarBlueServer.GetGame().GetCraftingManager().CraftingRecipes)
            {
                bool found = false;
                int total = 0;
                foreach (KeyValuePair<string, int> item in recipe.Value.ItemsNeeded)
                {

                    if (item.Value != items.Count(item2 => item2.GetBaseItem().ItemName == item.Key))
                    {
                        found = false;
                        break;
                    }
                    else
                    {
                        total = total + items.Count(item2 => item2.GetBaseItem().ItemName == item.Key);
                    }

                    if (total == items.Count)
                    {
                        found = true;
                    }
                }

                if (found == false)
                {
                    continue;
                }

                craftingRecipe = recipe.Value;
                break;
            }

            if (craftingRecipe == null || Session.GetHabbo().LastCraftingMachine != craftingRecipe.Type)
            {
                Session.SendMessage(new CraftingFoundComposer(0, false));
                return;
            }

            Session.SendMessage(new CraftingFoundComposer(0, true));

        }

    }
}

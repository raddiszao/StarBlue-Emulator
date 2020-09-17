using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftableProductsComposer : MessageComposer
    {
        private Item item { get; }

        public CraftableProductsComposer(Item item)
            : base(Composers.CraftableProductsMessageComposer)
        {
            this.item = item;
        }

        public override void Compose(Composer packet)
        {
            int total = 0;
            HabboHotel.Items.Crafting.CraftingManager crafting = StarBlueServer.GetGame().GetCraftingManager();
            foreach (HabboHotel.Items.Crafting.CraftingRecipe recipe in crafting.CraftingRecipes.Values)
            {
                if (recipe.Type == item.GetBaseItem().Id)
                {
                    int count = total + 1;
                    total = count;
                }
            }
            packet.WriteInteger(total); //crafting.CraftingRecipes.Count
            foreach (HabboHotel.Items.Crafting.CraftingRecipe recipe in crafting.CraftingRecipes.Values)
            {
                if (recipe.Type == item.GetBaseItem().Id)
                {
                    packet.WriteString(recipe.Result);
                    packet.WriteString(recipe.Result);
                }
            }
            packet.WriteInteger(crafting.CraftableItems.Count);
            foreach (string itemName in crafting.CraftableItems)
            {
                packet.WriteString(itemName);
            }
        }
    }
}
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftableProductsComposer : ServerPacket
    {
        public CraftableProductsComposer(Item item)
            : base(ServerPacketHeader.CraftableProductsMessageComposer)
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
            base.WriteInteger(total); //crafting.CraftingRecipes.Count
            foreach (HabboHotel.Items.Crafting.CraftingRecipe recipe in crafting.CraftingRecipes.Values)
            {
                if (recipe.Type == item.GetBaseItem().Id)
                {
                    base.WriteString(recipe.Result);
                    base.WriteString(recipe.Result);
                }
            }
            base.WriteInteger(crafting.CraftableItems.Count);
            foreach (string itemName in crafting.CraftableItems)
            {
                base.WriteString(itemName);
            }
        }
    }
}
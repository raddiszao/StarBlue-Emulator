using StarBlue.HabboHotel.Items.Crafting;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftingRecipeComposer : MessageComposer
    {
        private CraftingRecipe recipe { get; }

        public CraftingRecipeComposer(CraftingRecipe recipe) : base(Composers.CraftingRecipeMessageComposer)
        {
            this.recipe = recipe;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(recipe.ItemsNeeded.Count);
            foreach (System.Collections.Generic.KeyValuePair<string, int> item in recipe.ItemsNeeded)
            {
                packet.WriteInteger(item.Value);
                packet.WriteString(item.Key);
            }
        }
    }
}
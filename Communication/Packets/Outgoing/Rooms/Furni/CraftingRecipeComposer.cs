using StarBlue.HabboHotel.Items.Crafting;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftingRecipeComposer : ServerPacket
    {
        public CraftingRecipeComposer(CraftingRecipe recipe) : base(ServerPacketHeader.CraftingRecipeMessageComposer)
        {
            base.WriteInteger(recipe.ItemsNeeded.Count);
            foreach (System.Collections.Generic.KeyValuePair<string, int> item in recipe.ItemsNeeded)
            {
                base.WriteInteger(item.Value);
                base.WriteString(item.Key);
            }
        }
    }
}
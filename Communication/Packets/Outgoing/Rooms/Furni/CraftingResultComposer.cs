using StarBlue.HabboHotel.Items.Crafting;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class CraftingResultComposer : MessageComposer
    {
        private CraftingRecipe recipe { get; }
        private bool success { get; }

        public CraftingResultComposer(CraftingRecipe recipe, bool success)
            : base(Composers.CraftingResultMessageComposer)
        {
            this.recipe = recipe;
            this.success = success;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(success);
            packet.WriteString(recipe.Result);
            packet.WriteString(recipe.Result);
        }
    }
}
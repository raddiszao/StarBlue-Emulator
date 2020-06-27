using StarBlue.HabboHotel.Items.Crafting;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class CraftingResultComposer : ServerPacket
    {
        public CraftingResultComposer(CraftingRecipe recipe, bool success)
            : base(ServerPacketHeader.CraftingResultMessageComposer)
        {
            base.WriteBoolean(success);
            base.WriteString(recipe.Result);
            base.WriteString(recipe.Result);
        }
    }
}
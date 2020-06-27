namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    class GetCraftingItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //var result = Packet.PopString();

            //CraftingRecipe recipe = null;
            //foreach (CraftingRecipe Receta in StarBlueServer.GetGame().GetCraftingManager().CraftingRecipes.Values)
            //{
            //    if (Receta.Result.Contains(result))
            //    {
            //        recipe = Receta;
            //        break;
            //    }
            //}

            //var Final = StarBlueServer.GetGame().GetCraftingManager().GetRecipe(recipe.Id);

            //Session.SendMessage(new CraftingResultComposer(recipe, true));
            //Session.SendMessage(new CraftableProductsComposer());
        }
    }
}
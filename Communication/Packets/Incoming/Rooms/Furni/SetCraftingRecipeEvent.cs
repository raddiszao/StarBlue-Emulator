﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.HabboHotel.Items.Crafting;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    class SetCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            var result = Packet.PopString();

            CraftingRecipe recipe = null;
            foreach (CraftingRecipe Receta in StarBlueServer.GetGame().GetCraftingManager().CraftingRecipes.Values)
            {
                if (Receta.Result.Contains(result))
                {
                    recipe = Receta;
                    break;
                }
            }

            var Final = StarBlueServer.GetGame().GetCraftingManager().GetRecipe(recipe.Id);
            if (Final == null)
            {
                return;
            }

            Session.SendMessage(new CraftingRecipeComposer(Final));
        }

    }
}
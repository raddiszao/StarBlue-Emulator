using System;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Items.Crafting
{
    internal class CraftingRecipe
    {
        internal string Id;
        internal Dictionary<string, int> ItemsNeeded;
        internal string Result;
        internal int Type;
        public CraftingRecipe(string id, string itemsNeeded, string result, int type)
        {
            Id = id;
            ItemsNeeded = new Dictionary<string, int>();
            string[] splitted = itemsNeeded.Split(';');
            foreach (string split in splitted)
            {
                string[] item = split.Split(':');
                if (item.Length != 2)
                {
                    continue;
                }

                ItemsNeeded.Add(item[0], Convert.ToInt32(item[1]));
            }

            Type = type;
            Result = result;
        }
    }
}
using MoreLinq;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Furni
{
    internal class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            IEnumerable<Item> Items = Session.GetHabbo().GetInventoryComponent().GetWallAndFloor;
            int page = 0;
            int pages = ((Items.Count() - 1) / 700) + 1;
            if (Items.Count() > 3000)
            {
                Session.SendWhisper("Ei! Você já está chegando ao limite de itens no seu inventário! Tome cuidado, caso ele lote você poderá perder todos os itens e não nos responsabilizaremos pelos seus mobis perdidos.", 34);
            }

            if (Items.Count() == 0)
            {
                Session.SendMessage(new FurniListComposer(Items.ToList(), 1, 0));
            }
            else
            {
                foreach (ICollection<Item> batch in Items.Batch(700))
                {
                    Session.SendMessage(new FurniListComposer(batch.ToList(), pages, page));
                    page++;
                }
            }
        }

    }
}
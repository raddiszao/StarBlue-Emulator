using StarBlue.HabboHotel.Users.Inventory.Bots;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Bots
{
    internal class BotInventoryComposer : MessageComposer
    {
        public ICollection<Bot> Bots { get; }

        public BotInventoryComposer(ICollection<Bot> Bots)
            : base(Composers.BotInventoryMessageComposer)
        {
            this.Bots = Bots;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                packet.WriteInteger(Bot.Id);
                packet.WriteString(Bot.Name);
                packet.WriteString(Bot.Motto);
                packet.WriteString(Bot.Gender);
                packet.WriteString(Bot.Figure);
            }
        }
    }
}
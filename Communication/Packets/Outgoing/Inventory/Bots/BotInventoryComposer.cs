using StarBlue.HabboHotel.Users.Inventory.Bots;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Bots
{
    internal class BotInventoryComposer : ServerPacket
    {
        public BotInventoryComposer(ICollection<Bot> Bots)
            : base(ServerPacketHeader.BotInventoryMessageComposer)
        {
            base.WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                base.WriteInteger(Bot.Id);
                base.WriteString(Bot.Name);
                base.WriteString(Bot.Motto);
                base.WriteString(Bot.Gender);
                base.WriteString(Bot.Figure);
            }
        }
    }
}
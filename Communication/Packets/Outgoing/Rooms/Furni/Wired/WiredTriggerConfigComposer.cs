using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredTriggerConfigComposer : MessageComposer
    {
        public IWiredItem Box { get; }
        public List<int> BlockedItems { get; }

        public WiredTriggerConfigComposer(IWiredItem box, List<int> blockedItems)
            : base(Composers.WiredTriggerConfigMessageComposer)
        {
            this.Box = box;
            this.BlockedItems = blockedItems;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(20);

            packet.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                packet.WriteInteger(Item.Id);
            }

            packet.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(Box.Item.Id);
            packet.WriteString(Box.StringData);

            packet.WriteInteger(Box is IWiredCycle ? 1 : 0);
            if (Box is IWiredCycle)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                if (Box.Type == WiredBoxType.TriggerRepeat)
                {
                    packet.WriteInteger(Cycle.Delay / 500);
                }
                else if (Box.Type == WiredBoxType.TriggerLongRepeat)
                {
                    packet.WriteInteger(Cycle.Delay / 500 / 10000);
                }
                else
                {
                    packet.WriteInteger(Cycle.Delay);
                }
            }
            packet.WriteInteger(0);
            packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
            packet.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int Id in BlockedItems.ToList())
                {
                    packet.WriteInteger(Id);
                }
            }
        }
    }
}
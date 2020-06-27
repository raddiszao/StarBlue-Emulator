using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredTriggerConfigComposer : ServerPacket
    {
        public WiredTriggerConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(ServerPacketHeader.WiredTriggerConfigMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(15);

            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                base.WriteInteger(Item.Id);
            }

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);
            base.WriteString(Box.StringData);

            base.WriteInteger(Box is IWiredCycle ? 1 : 0);
            if (Box is IWiredCycle)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                if (Box.Type == WiredBoxType.TriggerRepeat)
                {
                    base.WriteInteger(Cycle.Delay / 500);
                }
                else if (Box.Type == WiredBoxType.TriggerLongRepeat)
                {
                    base.WriteInteger(Cycle.Delay / 500 / 10000);
                }
                else
                {
                    base.WriteInteger(Cycle.Delay);
                }
            }
            base.WriteInteger(0);
            base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
            base.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int Id in BlockedItems.ToList())
                {
                    base.WriteInteger(Id);
                }
            }
        }
    }
}
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredConditionConfigComposer : MessageComposer
    {
        public IWiredItem Box { get; }

        public WiredConditionConfigComposer(IWiredItem Box)
            : base(Composers.WiredConditionConfigMessageComposer)
        {
            this.Box = Box;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
            if (Box.Type == WiredBoxType.TotalUsersCoincidence) { packet.WriteInteger(25); }
            else
            {
                packet.WriteInteger(15);
            }

            packet.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                packet.WriteInteger(Item.Id);
            }

            packet.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(Box.Item.Id);
            packet.WriteString(Box.StringData);

            if (Box.Type == WiredBoxType.ConditionDateRangeActive)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                packet.WriteInteger(2);//Loop
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);

            }

            if (Box.Type == WiredBoxType.ConditionMatchStateAndPosition || Box.Type == WiredBoxType.ConditionDontMatchStateAndPosition)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0;0";
                }

                packet.WriteInteger(3);//Loop
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.ConditionUserCountInRoom || Box.Type == WiredBoxType.ConditionUserCountDoesntInRoom)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                packet.WriteInteger(2);//Loop
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 1);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 50);
            }

            if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni || Box.Type == WiredBoxType.ConditionFurniHasFurni)
            {
                packet.WriteInteger(1);
            }

            if (Box.Type != WiredBoxType.ConditionUserCountInRoom && Box.Type != WiredBoxType.ConditionUserCountDoesntInRoom && Box.Type != WiredBoxType.ConditionFurniHasNoFurni && Box.Type != WiredBoxType.ConditionFurniHasFurni && Box.Type != WiredBoxType.ConditionDateRangeActive)
            {
                packet.WriteInteger(0);
            }
            else if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni || Box.Type == WiredBoxType.ConditionFurniHasFurni)
            {
                packet.WriteInteger(Box.BoolData ? 1 : 0);
            }
            packet.WriteInteger(0);
            packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
        }
    }
}
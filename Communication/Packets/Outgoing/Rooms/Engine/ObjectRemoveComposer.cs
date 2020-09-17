
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectRemoveComposer : MessageComposer
    {
        public int Item { get; }
        public int UserId { get; }

        public ObjectRemoveComposer(Item Item, int UserId)
            : base(Composers.ObjectRemoveMessageComposer)
        {
            this.Item = Item.Id;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Item.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}
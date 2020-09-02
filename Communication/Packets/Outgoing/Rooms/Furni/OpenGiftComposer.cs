
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class OpenGiftComposer : ServerPacket
    {
        public OpenGiftComposer(ItemData Data, string Text, Item Item, bool ItemIsInRoom)
            : base(ServerPacketHeader.OpenGiftMessageComposer)
        {
            base.WriteString(Data.Type.ToString());
            base.WriteInteger(Data.SpriteId);
            base.WriteString(Data.ItemName);
            base.WriteInteger(Item.Id);
            base.WriteString(Data.Type.ToString());
            base.WriteBoolean(ItemIsInRoom);//Is it in the room?
            base.WriteString(Text);
        }
    }
}

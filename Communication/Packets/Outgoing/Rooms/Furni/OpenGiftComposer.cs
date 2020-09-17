
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class OpenGiftComposer : MessageComposer
    {
        public ItemData Data { get; }
        public string Text { get; }
        public Item Item { get; }
        public bool ItemIsInRoom { get; }

        public OpenGiftComposer(ItemData Data, string Text, Item Item, bool ItemIsInRoom)
            : base(Composers.OpenGiftMessageComposer)
        {
            this.Data = Data;
            this.Text = Text;
            this.Item = Item;
            this.ItemIsInRoom = ItemIsInRoom;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Data.Type.ToString());
            packet.WriteInteger(Data.SpriteId);
            packet.WriteString(Data.ItemName);
            packet.WriteInteger(Item.Id);
            packet.WriteString(Data.Type.ToString());
            packet.WriteBoolean(ItemIsInRoom);//Is it in the room?
            packet.WriteString(Text);
        }
    }
}

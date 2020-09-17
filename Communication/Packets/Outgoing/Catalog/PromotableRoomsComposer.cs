using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class PromotableRoomsComposer : MessageComposer
    {
        private ICollection<RoomData> Rooms { get; }

        public PromotableRoomsComposer(ICollection<RoomData> Rooms)
            : base(Composers.PromotableRoomsMessageComposer)
        {
            this.Rooms = Rooms;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(Rooms.Count);//Count

            foreach (RoomData Data in Rooms)
            {
                packet.WriteInteger(Data.Id);
                packet.WriteString(Data.Name);
                packet.WriteBoolean(false);
            }
        }
    }
}
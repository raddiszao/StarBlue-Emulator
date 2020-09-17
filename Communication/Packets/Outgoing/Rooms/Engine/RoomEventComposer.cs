using StarBlue.HabboHotel.Rooms;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomEventComposer : MessageComposer
    {
        public RoomData Data { get; }
        public RoomPromotion Promotion { get; }

        public RoomEventComposer(RoomData Data, RoomPromotion Promotion)
            : base(Composers.RoomEventMessageComposer)
        {
            this.Data = Data;
            this.Promotion = Promotion;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Promotion == null ? -1 : Convert.ToInt32(Data.Id));
            packet.WriteInteger(Promotion == null ? -1 : Data.OwnerId);
            packet.WriteString(Promotion == null ? "" : Data.OwnerName);
            packet.WriteInteger(Promotion == null ? 0 : 1);
            packet.WriteInteger(0);
            packet.WriteString(Promotion == null ? "" : Promotion.Name);
            packet.WriteString(Promotion == null ? "" : Promotion.Description);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0); //Unknown, came in build RELEASE63-201411181343-400753188

        }
    }
}

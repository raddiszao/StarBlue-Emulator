
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomInfoComposer : MessageComposer
    {
        private RoomData Data { get; }
        private bool OwnerInRoom { get; }

        public ModeratorRoomInfoComposer(RoomData Data, bool OwnerInRoom)
            : base(Composers.ModeratorRoomInfoMessageComposer)
        {
            this.Data = Data;
            this.OwnerInRoom = OwnerInRoom;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Data.Id);
            packet.WriteInteger(Data.UsersNow);
            packet.WriteBoolean(OwnerInRoom); // owner in room
            packet.WriteInteger(Data.OwnerId);
            packet.WriteString(Data.OwnerName);
            packet.WriteBoolean(Data != null);
            packet.WriteString(Data.Name);
            packet.WriteString(Data.Description);

            packet.WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
                packet.WriteString(Tag);
            }

            packet.WriteBoolean(false);
        }
    }
}

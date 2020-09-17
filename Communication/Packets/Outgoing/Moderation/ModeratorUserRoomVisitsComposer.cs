using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using StarBlue.Utilities;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserRoomVisitsComposer : MessageComposer
    {
        private Habbo Data { get; }
        private Dictionary<double, RoomData> Visits { get; }

        public ModeratorUserRoomVisitsComposer(Habbo Data, Dictionary<double, RoomData> Visits)
            : base(Composers.ModeratorUserRoomVisitsMessageComposer)
        {
            this.Data = Data;
            this.Visits = Visits;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Data.Id);
            packet.WriteString(Data.Username);
            packet.WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, RoomData> Visit in Visits)
            {
                packet.WriteInteger(Visit.Value.Id);
                packet.WriteString(Visit.Value.Name);
                packet.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                packet.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}

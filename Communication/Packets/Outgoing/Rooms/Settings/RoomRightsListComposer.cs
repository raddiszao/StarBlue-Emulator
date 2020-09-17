using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomRightsListComposer : MessageComposer
    {
        public Room Room { get; }
        public RoomRightsListComposer(Room Instance)
            : base(Composers.RoomRightsListMessageComposer)
        {
            this.Room = Instance;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Room.Id);

            packet.WriteInteger(Room.UsersWithRights.Count);
            foreach (int Id in Room.UsersWithRights.ToList())
            {
                UserCache Data = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Id);
                if (Data == null)
                {
                    packet.WriteInteger(0);
                    packet.WriteString("Unknown Error");
                }
                else
                {
                    packet.WriteInteger(Data.Id);
                    packet.WriteString(Data.Username);
                }
            }
        }
    }
}

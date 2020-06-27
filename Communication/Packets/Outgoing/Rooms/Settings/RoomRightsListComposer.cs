using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomRightsListComposer : ServerPacket
    {
        public RoomRightsListComposer(Room Instance)
            : base(ServerPacketHeader.RoomRightsListMessageComposer)
        {
            base.WriteInteger(Instance.Id);

            base.WriteInteger(Instance.UsersWithRights.Count);
            foreach (int Id in Instance.UsersWithRights.ToList())
            {
                UserCache Data = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Id);
                if (Data == null)
                {
                    base.WriteInteger(0);
                    base.WriteString("Unknown Error");
                }
                else
                {
                    base.WriteInteger(Data.Id);
                    base.WriteString(Data.Username);
                }
            }
        }
    }
}

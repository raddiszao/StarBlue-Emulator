using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room Instance)
            : base(ServerPacketHeader.GetRoomBannedUsersMessageComposer)
        {
            base.WriteInteger(Instance.Id);

            base.WriteInteger(Instance.BannedUsers().Count);//Count
            foreach (int Id in Instance.BannedUsers().ToList())
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

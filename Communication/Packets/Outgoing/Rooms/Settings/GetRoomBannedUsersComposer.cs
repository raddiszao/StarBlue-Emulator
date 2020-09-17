using StarBlue.HabboHotel.Cache;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class GetRoomBannedUsersComposer : MessageComposer
    {
        public int RoomId { get; }
        public List<int> BannedUsers { get; }
        public GetRoomBannedUsersComposer(int RoomId, List<int> BannedUsers)
            : base(Composers.GetRoomBannedUsersMessageComposer)
        {
            this.RoomId = RoomId;
            this.BannedUsers = BannedUsers;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);

            packet.WriteInteger(BannedUsers.Count);//Count
            foreach (int Id in BannedUsers)
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

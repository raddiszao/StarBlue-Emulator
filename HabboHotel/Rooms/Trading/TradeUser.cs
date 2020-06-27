using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Rooms.Trading
{

    public class TradeUser
    {
        public int UserId;
        private readonly int RoomId;
        public List<Item> OfferedItems;

        public TradeUser(int UserId, int RoomId)
        {
            this.UserId = UserId;
            this.RoomId = RoomId;
            HasAccepted = false;
            OfferedItems = new List<Item>();
        }

        public bool HasAccepted { get; set; }

        public RoomUser GetRoomUser()
        {

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room Room))
            {
                return null;
            }

            return Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
        }

        public GameClient GetClient()
        {
            return StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
        }
    }
}

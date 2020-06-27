using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(StarBlueServer.GetConfig().data["group.purchase.amount"]));//Price

            base.WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
                base.WriteInteger(Room.Id);//Room Id
                base.WriteString(Room.Name);//Room Name
                base.WriteBoolean(false);//What?
            }

            base.WriteInteger(5);
            base.WriteInteger(5);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(6);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}

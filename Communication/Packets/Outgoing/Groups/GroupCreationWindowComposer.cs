using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupCreationWindowComposer : MessageComposer
    {
        public ICollection<RoomData> Rooms { get; }

        public GroupCreationWindowComposer(ICollection<RoomData> rooms)
            : base(Composers.GroupCreationWindowMessageComposer)
        {
            this.Rooms = rooms;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Convert.ToInt32(StarBlueServer.GetConfig().data["group.purchase.amount"]));//Price

            packet.WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
                packet.WriteInteger(Room.Id);//Room Id
                packet.WriteString(Room.Name);//Room Name
                packet.WriteBoolean(false);//What?
            }

            packet.WriteInteger(5);
            packet.WriteInteger(5);
            packet.WriteInteger(11);
            packet.WriteInteger(4);

            packet.WriteInteger(6);
            packet.WriteInteger(11);
            packet.WriteInteger(4);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
        }
    }
}

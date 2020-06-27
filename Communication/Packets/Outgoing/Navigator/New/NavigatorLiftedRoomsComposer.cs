﻿namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    class NavigatorLiftedRoomsComposer : ServerPacket
    {
        public NavigatorLiftedRoomsComposer()
            : base(ServerPacketHeader.NavigatorLiftedRoomsMessageComposer)
        {
            base.WriteInteger(0);//Count
            {
                base.WriteInteger(1);//Flat Id
                base.WriteInteger(0);//Unknown
                base.WriteString("");//Image
                base.WriteString("Caption");//Caption.
            }
        }
    }
}

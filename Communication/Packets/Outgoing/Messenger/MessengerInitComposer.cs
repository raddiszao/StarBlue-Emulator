using System;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer(int Rank)
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            int FriendsLimit = Convert.ToInt32(StarBlueServer.GetConfig().data["messenger.friend.limit"]);
            if (Rank > 1)
            {
                FriendsLimit = Convert.ToInt32(StarBlueServer.GetConfig().data["messenger.vip.friend.limit"]);
            }

            base.WriteInteger(FriendsLimit);//Friends max.
            base.WriteInteger(300);
            base.WriteInteger(800);
            base.WriteInteger(1); // category count
            base.WriteInteger(1);//category id
            base.WriteString("Grupos");//category name
        }
    }
}

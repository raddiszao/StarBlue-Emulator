using System;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class MessengerInitComposer : MessageComposer
    {
        private int Rank { get; }

        public MessengerInitComposer(int Rank)
            : base(Composers.MessengerInitMessageComposer)
        {
            this.Rank = Rank;
        }

        public override void Compose(Composer packet)
        {
            int FriendsLimit = Convert.ToInt32(StarBlueServer.GetConfig().data["messenger.friend.limit"]);
            if (Rank > 1)
            {
                FriendsLimit = Convert.ToInt32(StarBlueServer.GetConfig().data["messenger.vip.friend.limit"]);
            }

            packet.WriteInteger(FriendsLimit);//Friends max.
            packet.WriteInteger(300);
            packet.WriteInteger(800);
            packet.WriteInteger(1); // category count
            packet.WriteInteger(1);//category id
            packet.WriteString("Grupos");//category name
        }
    }
}

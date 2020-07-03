using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    class UserTagsComposer : ServerPacket
    {
        public UserTagsComposer(int UserId, GameClient Session)
            : base(ServerPacketHeader.UserTagsMessageComposer)
        {
            WriteInteger(UserId);
            WriteInteger(0);//Count of the tags.
            {
                //Append a string.
            }

            /*if (Session.GetHabbo().Tags != null)
            {
                base.WriteInteger(UserId);
                base.WriteInteger(Session.GetHabbo().Tags.Count);//Count of the tags.
                foreach (string tag in Session.GetHabbo().Tags.ToArray())
                {
                    base.WriteString(tag);
                }
            }*/
        }
    }
}

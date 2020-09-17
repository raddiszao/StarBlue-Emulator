using StarBlue.HabboHotel.Users.Messenger;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FriendNotificationComposer : MessageComposer
    {
        public int UserId { get; }
        public MessengerEventTypes Type { get; }
        public string Data { get; }

        public FriendNotificationComposer(int UserId, MessengerEventTypes type, string data)
            : base(Composers.FriendNotificationMessageComposer)
        {
            this.UserId = UserId;
            this.Type = type;
            this.Data = data;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(UserId.ToString());
            packet.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(Type));
            packet.WriteString(Data);
        }
    }
}

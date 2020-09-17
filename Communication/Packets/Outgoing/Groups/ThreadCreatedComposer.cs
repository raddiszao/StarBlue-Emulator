using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    public class ThreadCreatedComposer : MessageComposer
    {
        private GroupForumThread Thread { get; }
        private GameClient Session { get; }

        public ThreadCreatedComposer(GameClient Session, GroupForumThread Thread)
            : base(Composers.ThreadCreatedMessageComposer)
        {
            this.Session = Session;
            this.Thread = Thread;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Thread.ParentForum.Id); //Thread ID
            Thread.SerializeData(Session, packet);
            /*
            packet.WriteInteger(Thread.Id); //Thread ID
            packet.WriteInteger(Thread.GetAuthor().Id);
            packet.WriteString(Thread.GetAuthor().Username); //Thread Author
            packet.WriteString(Thread.Caption); //Thread Title
            packet.WriteBoolean(false); //Pinned
            packet.WriteBoolean(false); //Locked
            packet.WriteInteger((int)(StarBlueServer.GetUnixTimestamp() - Thread.Timestamp)); //Created Secs Ago
            packet.WriteInteger(Thread.Posts.Count); //Message count
            packet.WriteInteger(Thread.GetUnreadMessages(Session.GetHabbo().Id)); //Unread message count
            packet.WriteInteger(0); // idk
            packet.WriteInteger(0); // idk

            packet.WriteString("Unknown");// Last User Post Username
            packet.WriteInteger(65); // Last User Post time ago [Sec]

            packet.WriteByte(0); //idk
            packet.WriteInteger(10);// idk
            packet.WriteString("Str4"); //idk
            packet.WriteInteger(11);//idk*/
        }
    }
}

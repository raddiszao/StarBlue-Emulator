using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadUpdatedComposer : MessageComposer
    {
        private GameClient Session { get; }
        private GroupForumThread Thread { get; }

        public ThreadUpdatedComposer(GameClient Session, GroupForumThread Thread)
            : base(Composers.ThreadUpdatedMessageComposer)
        {
            this.Session = Session;
            this.Thread = Thread;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Thread.ParentForum.Id);

            Thread.SerializeData(Session, packet);
        }
    }
}

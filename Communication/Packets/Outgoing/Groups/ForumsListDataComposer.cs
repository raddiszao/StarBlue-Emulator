using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ForumsListDataComposer : MessageComposer
    {
        private ICollection<GroupForum> Forums { get; }
        private GameClient Session { get; }
        private int ViewOrder { get; }
        private int StartIndex { get; }
        private int MaxLength { get; }

        public ForumsListDataComposer(ICollection<GroupForum> Forums, GameClient Session, int ViewOrder = 0, int StartIndex = 0, int MaxLength = 20)
            : base(Composers.ForumsListDataMessageComposer)
        {
            this.Forums = Forums;
            this.Session = Session;
            this.ViewOrder = ViewOrder;
            this.StartIndex = StartIndex;
            this.MaxLength = MaxLength;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ViewOrder);
            packet.WriteInteger(StartIndex);
            packet.WriteInteger(StartIndex);

            packet.WriteInteger(Forums.Count); // Forum List Count

            foreach (GroupForum Forum in Forums)
            {
                GroupForumThreadPost lastpost = Forum.GetLastPost();
                bool isn = lastpost == null;
                packet.WriteInteger(Forum.Id); //Maybe ID
                packet.WriteString(Forum.Name); //Forum name
                packet.WriteString(Forum.Description); //idk
                packet.WriteString(Forum.Group.Badge); // Group Badge
                packet.WriteInteger(0);//Idk
                packet.WriteInteger(0);// Score
                packet.WriteInteger(Forum.MessagesCount);//Message count
                packet.WriteInteger(Forum.UnreadMessages(Session.GetHabbo().Id));//unread message count
                packet.WriteInteger(0);//Idk
                packet.WriteInteger(!isn ? lastpost.GetAuthor().Id : 0);// Las user to message id
                packet.WriteString(!isn ? lastpost.GetAuthor().Username : ""); //Last user to message name
                packet.WriteInteger(!isn ? (int)StarBlueServer.GetUnixTimestamp() - lastpost.Timestamp : 0); //Last message timestamp
            }
        }
    }
}

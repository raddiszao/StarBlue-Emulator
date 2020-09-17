using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GetGroupForumsMessageEvent : MessageComposer
    {
        private GroupForum Forum { get; }
        private GameClient Session { get; }

        public GetGroupForumsMessageEvent(GroupForum Forum, GameClient Session)
            : base(Composers.GroupForumDataMessageComposer)
        {
            this.Forum = Forum;
            this.Session = Session;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Forum.Id);
            packet.WriteString(Forum.Group.Name); //Group Name
            packet.WriteString(Forum.Group.Description); // idk
            packet.WriteString(Forum.Group.Badge); //Group Badge code

            packet.WriteInteger(Forum.Threads.Count); //Forum Thread Count
            packet.WriteInteger(0); //Last Author ID
            packet.WriteInteger(0); //Score ?
            packet.WriteInteger(0); //Last Thread Mark
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteString("not_member");
            packet.WriteInteger(0);
            //end fillFromMEssage func

            packet.WriteInteger(Forum.Settings.WhoCanRead); //Who can read 
            packet.WriteInteger(Forum.Settings.WhoCanPost); // Who can post
            packet.WriteInteger(Forum.Settings.WhoCanInitDiscussions); //Who can make threads
            packet.WriteInteger(Forum.Settings.WhoCanModerate); //Who can MOD

            //Permissions i think
            packet.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); //Forum disabled reason//packet.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); 
            packet.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)); //Can't reply reason
            packet.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions));// Can't Post reason
            packet.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate)); //Can't moderate thread posts reason
            packet.WriteString("");

            packet.WriteBoolean(Forum.Group.CreatorId == Session.GetHabbo().Id); // Is Owner
            packet.WriteBoolean(Forum.Group.IsAdmin(Session.GetHabbo().Id) && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) == ""); // Is admin

        }
    }
}

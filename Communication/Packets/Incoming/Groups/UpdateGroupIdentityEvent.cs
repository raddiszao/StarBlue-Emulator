
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class UpdateGroupIdentityEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            string Name = Packet.PopString();
            Name = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word) ? "Spam" : Name;
            string Desc = Packet.PopString();
            Desc = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id)
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = '" + GroupId + "' LIMIT 1");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.RunQuery();
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendMessage(new GroupInfoComposer(Group, Session));
        }
    }
}

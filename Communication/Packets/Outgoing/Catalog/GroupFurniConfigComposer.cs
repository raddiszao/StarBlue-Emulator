using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class GroupFurniConfigComposer : MessageComposer
    {
        private ICollection<Group> Groups { get; }

        public GroupFurniConfigComposer(ICollection<Group> Groups)
            : base(Composers.GroupFurniConfigMessageComposer)
        {
            this.Groups = Groups;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                packet.WriteInteger(Group.Id);
                packet.WriteString(Group.Name);
                packet.WriteString(Group.Badge);
                packet.WriteString((StarBlueServer.GetGame().GetGroupManager().SymbolColours.ContainsKey(Group.Colour1)) ? StarBlueServer.GetGame().GetGroupManager().SymbolColours[Group.Colour1].Colour : "4f8a00"); // Group Colour 1
                packet.WriteString((StarBlueServer.GetGame().GetGroupManager().BackGroundColours.ContainsKey(Group.Colour2)) ? StarBlueServer.GetGame().GetGroupManager().BackGroundColours[Group.Colour2].Colour : "4f8a00"); // Group Colour 2            
                packet.WriteBoolean(false);
                packet.WriteInteger(Group.CreatorId);
                packet.WriteBoolean(Group.ForumEnabled);
            }
        }
    }
}

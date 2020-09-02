using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class GroupFurniConfigComposer : ServerPacket
    {
        public GroupFurniConfigComposer(ICollection<Group> Groups)
            : base(ServerPacketHeader.GroupFurniConfigMessageComposer)
        {
            base.WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                base.WriteInteger(Group.Id);
                base.WriteString(Group.Name);
                base.WriteString(Group.Badge);
                base.WriteString((StarBlueServer.GetGame().GetGroupManager().SymbolColours.ContainsKey(Group.Colour1)) ? StarBlueServer.GetGame().GetGroupManager().SymbolColours[Group.Colour1].Colour : "4f8a00"); // Group Colour 1
                base.WriteString((StarBlueServer.GetGame().GetGroupManager().BackGroundColours.ContainsKey(Group.Colour2)) ? StarBlueServer.GetGame().GetGroupManager().BackGroundColours[Group.Colour2].Colour : "4f8a00"); // Group Colour 2            
                base.WriteBoolean(false);
                base.WriteInteger(Group.CreatorId);
                base.WriteBoolean(Group.ForumEnabled);
            }
        }
    }
}

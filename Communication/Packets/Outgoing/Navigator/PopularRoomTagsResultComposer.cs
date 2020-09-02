using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class PopularRoomTagsResultComposer : ServerPacket
    {
        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> Tags)
            : base(ServerPacketHeader.PopularRoomTagsResultMessageComposer)
        {
            base.WriteInteger(Tags.Count);
            foreach (KeyValuePair<string, int> tag in Tags)
            {
                base.WriteString(tag.Key);
                base.WriteInteger(tag.Value);
            }
        }
    }
}

using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class PopularRoomTagsResultComposer : MessageComposer
    {
        private ICollection<KeyValuePair<string, int>> Tags { get; }

        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> Tags)
            : base(Composers.PopularRoomTagsResultMessageComposer)
        {
            this.Tags = Tags;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Tags.Count);
            foreach (KeyValuePair<string, int> tag in Tags)
            {
                packet.WriteString(tag.Key);
                packet.WriteInteger(tag.Value);
            }
        }
    }
}

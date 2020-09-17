using StarBlue.HabboHotel.Items.Televisions;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    internal class GetYouTubePlaylistComposer : MessageComposer
    {
        public int ItemId { get; }
        public ICollection<TelevisionItem> Videos { get; }

        public GetYouTubePlaylistComposer(int ItemId, ICollection<TelevisionItem> Videos)
            : base(Composers.GetYouTubePlaylistMessageComposer)
        {
            this.ItemId = ItemId;
            this.Videos = Videos;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteInteger(Videos.Count);
            foreach (TelevisionItem Video in Videos.ToList())
            {
                packet.WriteString(Video.YouTubeId);
                packet.WriteString(Video.Title);//Title
                packet.WriteString(Video.Description);//Description
            }
            packet.WriteString("");
        }
    }
}

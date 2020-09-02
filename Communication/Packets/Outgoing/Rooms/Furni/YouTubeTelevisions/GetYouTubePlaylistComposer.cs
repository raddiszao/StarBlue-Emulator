using StarBlue.HabboHotel.Items.Televisions;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    internal class GetYouTubePlaylistComposer : ServerPacket
    {
        public GetYouTubePlaylistComposer(int ItemId, ICollection<TelevisionItem> Videos)
            : base(ServerPacketHeader.GetYouTubePlaylistMessageComposer)
        {
            base.WriteInteger(ItemId);
            base.WriteInteger(Videos.Count);
            foreach (TelevisionItem Video in Videos.ToList())
            {
                base.WriteString(Video.YouTubeId);
                base.WriteString(Video.Title);//Title
                base.WriteString(Video.Description);//Description
            }
            base.WriteString("");
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;
using StarBlue.HabboHotel.Items.Televisions;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    internal class YouTubeVideoInformationEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string VideoId = Packet.PopString();

            foreach (TelevisionItem Tele in StarBlueServer.GetGame().GetTelevisionManager().TelevisionList.ToList())
            {
                if (Tele.YouTubeId != VideoId)
                {
                    continue;
                }

                Session.SendMessage(new GetYouTubeVideoComposer(ItemId, Tele.YouTubeId));
            }
        }
    }
}
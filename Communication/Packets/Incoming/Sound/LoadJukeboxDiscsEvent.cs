using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class LoadJukeboxDiscsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
            {
                Session.SendMessage(new LoadJukeboxUserMusicItemsComposer(Session.GetHabbo().CurrentRoom));
            }
        }
    }
}

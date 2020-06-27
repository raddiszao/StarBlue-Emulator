using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.TraxMachine;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    class GetJukeboxDiscsDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var songslen = Packet.PopInt();
            var Songs = new List<TraxMusicData>();
            while (songslen-- > 0)
            {
                var id = Packet.PopInt();
                var music = TraxSoundManager.GetMusic(id);
                if (music != null)
                {
                    Songs.Add(music);
                }
            }
            if (Session.GetHabbo().CurrentRoom != null)
            {
                Session.SendMessage(new SetJukeboxSongMusicDataComposer(Songs));
            }
        }
    }
}

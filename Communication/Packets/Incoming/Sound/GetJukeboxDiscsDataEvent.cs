using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.TraxMachine;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class GetJukeboxDiscsDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int songslen = Packet.PopInt();
            List<TraxMusicData> Songs = new List<TraxMusicData>();
            while (songslen-- > 0)
            {
                int id = Packet.PopInt();
                TraxMusicData music = TraxSoundManager.GetMusic(id);
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

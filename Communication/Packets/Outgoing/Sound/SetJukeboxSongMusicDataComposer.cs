using StarBlue.HabboHotel.Rooms.TraxMachine;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SetJukeboxSongMusicDataComposer : MessageComposer
    {
        private ICollection<TraxMusicData> Songs { get; }

        public SetJukeboxSongMusicDataComposer(ICollection<TraxMusicData> Songs)
            : base(Composers.SetJukeboxSongMusicDataMessageComposer)
        {
            this.Songs = Songs;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Songs.Count);//while

            foreach (TraxMusicData item in Songs)
            {
                packet.WriteInteger(item.Id);// Song id
                packet.WriteString(item.CodeName); // Song code name
                packet.WriteString(item.Name);
                packet.WriteString(item.Data);
                packet.WriteInteger((int)(item.Length * 1000.0)); // Music Length - Duration
                packet.WriteString(item.Artist);
            }
        }
    }
}
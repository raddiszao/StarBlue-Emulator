using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SetJukeboxNowPlayingComposer : MessageComposer
    {
        private Room room { get; }

        public SetJukeboxNowPlayingComposer(Room room)
            : base(Composers.SetJukeboxNowPlayingMessageComposer)
        {
            this.room = room;
        }

        public override void Compose(Composer packet)
        {
            HabboHotel.Rooms.TraxMachine.RoomTraxManager trax = room.GetTraxManager();
            if (trax.IsPlaying && trax.ActualSongData != null)
            {

                HabboHotel.Items.Item actualmusicitem = trax.ActualSongData;
                HabboHotel.Rooms.TraxMachine.TraxMusicData actualmusic = trax.GetMusicByItem(actualmusicitem);
                int musicindex = trax.GetMusicIndex(actualmusicitem);
                int anteriorlength = trax.AnteriorMusic != null ? trax.AnteriorMusic.Length : 0;
                packet.WriteInteger(actualmusic.Id); // songid
                packet.WriteInteger(musicindex);
                packet.WriteInteger(actualmusic.Id); // songid
                packet.WriteInteger((int)((trax.TotalPlayListLength) * 1000.0));
                packet.WriteInteger((int)(trax.ActualSongTimePassed * 1000.0));
            }
            else
            {
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);

            }
        }
    }
}
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    class SetJukeboxNowPlayingComposer : ServerPacket
    {
        public SetJukeboxNowPlayingComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxNowPlayingMessageComposer)
        {
            var trax = room.GetTraxManager();
            if (trax.IsPlaying && trax.ActualSongData != null)
            {

                var actualmusicitem = trax.ActualSongData;
                var actualmusic = trax.GetMusicByItem(actualmusicitem);
                var musicindex = trax.GetMusicIndex(actualmusicitem);
                var anteriorlength = trax.AnteriorMusic != null ? trax.AnteriorMusic.Length : 0;
                base.WriteInteger(actualmusic.Id); // songid
                base.WriteInteger(musicindex);
                base.WriteInteger(actualmusic.Id); // songid
                base.WriteInteger((int)((trax.TotalPlayListLength) * 1000.0));
                base.WriteInteger((int)(trax.ActualSongTimePassed * 1000.0));
            }
            else
            {
                base.WriteInteger(-1);
                base.WriteInteger(-1);
                base.WriteInteger(-1);
                base.WriteInteger(-1);
                base.WriteInteger(-1);

            }
        }
    }
}
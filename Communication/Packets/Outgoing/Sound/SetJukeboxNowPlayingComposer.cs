using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SetJukeboxNowPlayingComposer : ServerPacket
    {
        public SetJukeboxNowPlayingComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxNowPlayingMessageComposer)
        {
            HabboHotel.Rooms.TraxMachine.RoomTraxManager trax = room.GetTraxManager();
            if (trax.IsPlaying && trax.ActualSongData != null)
            {

                HabboHotel.Items.Item actualmusicitem = trax.ActualSongData;
                HabboHotel.Rooms.TraxMachine.TraxMusicData actualmusic = trax.GetMusicByItem(actualmusicitem);
                int musicindex = trax.GetMusicIndex(actualmusicitem);
                int anteriorlength = trax.AnteriorMusic != null ? trax.AnteriorMusic.Length : 0;
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
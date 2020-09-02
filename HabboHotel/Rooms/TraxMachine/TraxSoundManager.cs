using log4net;
using StarBlue.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Rooms.TraxMachine
{
    public class TraxSoundManager
    {
        public static List<TraxMusicData> Songs = new List<TraxMusicData>();

        private static ILog Log = LogManager.GetLogger("StarBlue.HabboHotel.Rooms.TraxMachine");
        public static void Init()
        {
            Songs.Clear();

            DataTable table;
            using (IQueryAdapter adap = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.RunFastQuery("SELECT * FROM jukebox_songs_data");
                table = adap.GetTable();
            }

            foreach (DataRow row in table.Rows)
            {
                Songs.Add(TraxMusicData.Parse(row));
            }

            Log.Info(">> Jukebox Manager -> READY!");
        }

        public static TraxMusicData GetMusic(int id)
        {
            foreach (TraxMusicData item in Songs)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}

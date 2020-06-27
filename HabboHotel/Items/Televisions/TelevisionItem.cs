using System;

namespace StarBlue.HabboHotel.Items.Televisions
{
    public class TelevisionItem
    {
        private int _id;
        private string _youtubeId;
        private string _title;
        private string _description;
        private Boolean _enabled;

        public TelevisionItem(int Id, string YouTubeId, string Title, string Description, Boolean Enabled)
        {
            _id = Id;
            _youtubeId = YouTubeId;
            _title = Title;
            _description = Description;
            _enabled = Enabled;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string YouTubeId
        {
            get
            {
                return _youtubeId;
            }
        }


        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}

namespace StarBlue.HabboHotel.Items.Televisions
{
    public class TelevisionItem
    {
        private int _id;
        private string _youtubeId;
        private string _title;
        private string _description;
        private bool _enabled;

        public TelevisionItem(int Id, string YouTubeId, string Title, string Description, bool Enabled)
        {
            _id = Id;
            _youtubeId = YouTubeId;
            _title = Title;
            _description = Description;
            _enabled = Enabled;
        }

        public int Id => _id;

        public string YouTubeId => _youtubeId;


        public string Title => _title;

        public string Description => _description;

        public bool Enabled => _enabled;
    }
}

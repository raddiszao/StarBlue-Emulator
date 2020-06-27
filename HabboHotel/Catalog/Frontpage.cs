namespace StarBlue.HabboHotel.Catalog
{
    public class Frontpage
    {
        public string _frontImage;
        public string _frontLink;
        public string _frontName;
        public int _id;

        public Frontpage(int Id, string FrontName, string FrontLink, string FrontImage)
        {
            _id = Id;
            _frontName = FrontName;
            _frontLink = FrontLink;
            _frontImage = FrontImage;
        }

        public string FrontImage()
        {
            return _frontImage;
        }

        public string FrontLink()
        {
            return _frontLink;
        }

        public string FrontName()
        {
            return _frontName;
        }

        public int Id()
        {
            return _id;
        }
    }
}


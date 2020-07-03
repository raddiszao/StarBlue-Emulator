namespace StarBlue.HabboHotel.Users.Clothing.Parts
{
    public sealed class ClothingParts
    {
        private int _id;
        private int _partId;
        private string _part;

        public ClothingParts(int Id, int PartId, string Part)
        {
            _id = Id;
            _partId = PartId;
            _part = Part;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int PartId
        {
            get => _partId;
            set => _partId = value;
        }

        public string Part
        {
            get => _part;
            set => _part = value;
        }
    }
}
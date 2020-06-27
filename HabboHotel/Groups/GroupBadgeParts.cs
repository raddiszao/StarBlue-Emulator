namespace StarBlue.HabboHotel.Groups
{
    public class GroupBadgeParts
    {
        public int Id { get; private set; }
        public string AssetOne { get; private set; }
        public string AssetTwo { get; private set; }
        public GroupBadgeParts(int id, string assetOne, string assetTwo)
        {
            Id = id;
            AssetOne = assetOne;
            AssetTwo = assetTwo;
        }
    }
}
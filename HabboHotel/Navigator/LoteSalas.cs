namespace StarBlue.HabboHotel.Navigator
{
    public class LoteSalas
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public LoteSalas(int roomId, string image)
        {
            this.roomId = roomId;
            Image = image;
        }
    }
}

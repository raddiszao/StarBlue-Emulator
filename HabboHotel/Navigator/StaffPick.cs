namespace StarBlue.HabboHotel.Navigator
{
    public class StaffPick
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public StaffPick(int roomId, string image)
        {
            this.roomId = roomId;
            Image = image;
        }
    }
}

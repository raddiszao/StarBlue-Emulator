namespace StarBlue.HabboHotel.Groups
{
    public class GroupMember
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Look { get; set; }

        public string JoinedTime { get; set; }

        public GroupMember(int Id, string Username, string Look, string JoinedTime)
        {
            this.Id = Id;
            this.Username = Username;
            this.Look = Look;
            this.JoinedTime = JoinedTime;
        }
    }
}

﻿namespace StarBlue.HabboHotel.Users.Relationships
{
    public class Relationship
    {
        public int Id;
        public int Type;
        public int UserId;

        public Relationship(int Id, int User, int Type)
        {
            this.Id = Id;
            UserId = User;
            this.Type = Type;
        }
    }
}
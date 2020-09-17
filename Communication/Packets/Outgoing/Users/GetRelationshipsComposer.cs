
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Relationships;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class GetRelationshipsComposer : MessageComposer
    {
        private Habbo Habbo { get; }
        private int Loves { get; }
        private int Likes { get; }
        private int Hates { get; }


        public GetRelationshipsComposer(Habbo Habbo, int Loves, int Likes, int Hates)
            : base(Composers.GetRelationshipsMessageComposer)
        {
            this.Habbo = Habbo;
            this.Loves = Loves;
            this.Likes = Likes;
            this.Hates = Hates;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Habbo.Id);
            packet.WriteInteger(Habbo.Relationships.Count); // Count
            foreach (Relationship Rel in Habbo.Relationships.Values)
            {
                UserCache HHab = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Rel.UserId);
                if (HHab == null)
                {
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteInteger(0); // Their ID
                    packet.WriteString("Placeholder");
                    packet.WriteString("hr-115-42.hd-190-1.ch-215-62.lg-285-91.sh-290-62");
                }
                else
                {
                    packet.WriteInteger(Rel.Type);
                    packet.WriteInteger(Rel.Type == 1 ? Loves : Rel.Type == 2 ? Likes : Hates);
                    packet.WriteInteger(Rel.UserId); // Their ID
                    packet.WriteString(HHab.Username);
                    packet.WriteString(HHab.Look);
                }
            }
        }
    }
}

using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ManageGroupComposer : MessageComposer
    {
        public Group Group { get; }
        public string[] BadgeParts { get; }

        public ManageGroupComposer(Group Group, string[] BadgeParts)
            : base(Composers.ManageGroupMessageComposer)
        {
            this.Group = Group;
            this.BadgeParts = BadgeParts;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
            packet.WriteBoolean(true);
            packet.WriteInteger(Group.Id);
            packet.WriteString(Group.Name);
            packet.WriteString(Group.Description);
            packet.WriteInteger(1);
            packet.WriteInteger(Group.Colour1);
            packet.WriteInteger(Group.Colour2);
            packet.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            packet.WriteInteger(Group.AdminOnlyDeco);
            packet.WriteBoolean(false);
            packet.WriteString("");

            string[] BadgeSplit = Group.Badge.Replace("b", "").Split('s');
            packet.WriteInteger(5);
            int Req = 5 - BadgeSplit.Length;
            int Final = 0;
            string[] array2 = BadgeSplit;
            for (int i = 0; i < array2.Length; i++)
            {
                string Symbol = array2[i];
                packet.WriteInteger((Symbol.Length >= 6) ? int.Parse(Symbol.Substring(0, 3)) : int.Parse(Symbol.Substring(0, 2)));
                packet.WriteInteger((Symbol.Length >= 6) ? int.Parse(Symbol.Substring(3, 2)) : int.Parse(Symbol.Substring(2, 2)));
                packet.WriteInteger(Symbol.Length < 5 ? 0 : Symbol.Length >= 6 ? int.Parse(Symbol.Substring(5, 1)) : int.Parse(Symbol.Substring(4, 1)));
            }

            while (Final != Req)
            {
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                Final++;
            }

            packet.WriteString(Group.Badge);
            packet.WriteInteger(Group.MemberCount);
        }
    }
}

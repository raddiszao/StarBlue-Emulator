﻿
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ManageGroupComposer : ServerPacket
    {
        public ManageGroupComposer(Group Group)
            : base(ServerPacketHeader.ManageGroupMessageComposer)
        {
            base.WriteInteger(0);
            base.WriteBoolean(true);
            base.WriteInteger(Group.Id);
            base.WriteString(Group.Name);
            base.WriteString(Group.Description);
            base.WriteInteger(1);
            base.WriteInteger(Group.Colour1);
            base.WriteInteger(Group.Colour2);
            base.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            base.WriteInteger(Group.AdminOnlyDeco);
            base.WriteBoolean(false);
            base.WriteString("");

            string[] BadgeSplit = Group.Badge.Replace("b", "").Split('s');
            WriteInteger(5);
            int Req = 5 - BadgeSplit.Length;
            int Final = 0;
            string[] array2 = BadgeSplit;
            for (int i = 0; i < array2.Length; i++)
            {
                string Symbol = array2[i];
                WriteInteger((Symbol.Length >= 6) ? int.Parse(Symbol.Substring(0, 3)) : int.Parse(Symbol.Substring(0, 2)));
                WriteInteger((Symbol.Length >= 6) ? int.Parse(Symbol.Substring(3, 2)) : int.Parse(Symbol.Substring(2, 2)));
                WriteInteger(Symbol.Length < 5 ? 0 : Symbol.Length >= 6 ? int.Parse(Symbol.Substring(5, 1)) : int.Parse(Symbol.Substring(4, 1)));
            }

            while (Final != Req)
            {
                WriteInteger(0);
                WriteInteger(0);
                WriteInteger(0);
                Final++;
            }

            base.WriteString(Group.Badge);
            base.WriteInteger(Group.MemberCount);
        }
    }
}

﻿using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class BadgeEditorPartsComposer : ServerPacket
    {
        public BadgeEditorPartsComposer(ICollection<GroupBases> Bases, ICollection<GroupSymbols> Symbols, ICollection<GroupBaseColours> BaseColours, Dictionary<int, GroupSymbolColours> SymbolColours,
            Dictionary<int, GroupBackGroundColours> BackgroundColours)
            : base(ServerPacketHeader.BadgeEditorPartsMessageComposer)
        {
            base.WriteInteger(Bases.Count);
            foreach (GroupBases Item in Bases)
            {
                base.WriteInteger(Item.Id);
                base.WriteString(Item.Value1);
                base.WriteString(Item.Value2);
            }

            base.WriteInteger(Symbols.Count);
            foreach (GroupSymbols Item in Symbols)
            {
                base.WriteInteger(Item.Id);
                base.WriteString(Item.Value1);
                base.WriteString(Item.Value2);
            }

            base.WriteInteger(BaseColours.Count);
            foreach (GroupBaseColours Colour in BaseColours)
            {
                base.WriteInteger(Colour.Id);
                base.WriteString(Colour.Colour);
            }

            base.WriteInteger(SymbolColours.Count);
            foreach (GroupSymbolColours Colour in SymbolColours.Values.ToList())
            {
                base.WriteInteger(Colour.Id);
                base.WriteString(Colour.Colour);
            }

            base.WriteInteger(BackgroundColours.Count);
            foreach (GroupBackGroundColours Colour in BackgroundColours.Values.ToList())
            {
                base.WriteInteger(Colour.Id);
                base.WriteString(Colour.Colour);
            }
        }
    }
}
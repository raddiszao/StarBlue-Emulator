using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class BadgeEditorPartsComposer : MessageComposer
    {
        public ICollection<GroupBases> Bases { get; }
        public ICollection<GroupSymbols> Symbols { get; }
        public ICollection<GroupBaseColours> BaseColours { get; }
        public Dictionary<int, GroupSymbolColours> SymbolColours { get; }
        public Dictionary<int, GroupBackGroundColours> BackgroundColours { get; }

        public BadgeEditorPartsComposer(ICollection<GroupBases> Bases, ICollection<GroupSymbols> Symbols, ICollection<GroupBaseColours> BaseColours, Dictionary<int, GroupSymbolColours> SymbolColours,
          Dictionary<int, GroupBackGroundColours> BackgroundColours)
          : base(Composers.BadgeEditorPartsMessageComposer)
        {
            this.Bases = Bases;
            this.Symbols = Symbols;
            this.BaseColours = BaseColours;
            this.SymbolColours = SymbolColours;
            this.BackgroundColours = BackgroundColours;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Bases.Count);
            foreach (GroupBases Item in Bases)
            {
                packet.WriteInteger(Item.Id);
                packet.WriteString(Item.Value1);
                packet.WriteString(Item.Value2);
            }

            packet.WriteInteger(Symbols.Count);
            foreach (GroupSymbols Item in Symbols)
            {
                packet.WriteInteger(Item.Id);
                packet.WriteString(Item.Value1);
                packet.WriteString(Item.Value2);
            }

            packet.WriteInteger(BaseColours.Count);
            foreach (GroupBaseColours Colour in BaseColours)
            {
                packet.WriteInteger(Colour.Id);
                packet.WriteString(Colour.Colour);
            }

            packet.WriteInteger(SymbolColours.Count);
            foreach (GroupSymbolColours Colour in SymbolColours.Values.ToList())
            {
                packet.WriteInteger(Colour.Id);
                packet.WriteString(Colour.Colour);
            }

            packet.WriteInteger(BackgroundColours.Count);
            foreach (GroupBackGroundColours Colour in BackgroundColours.Values.ToList())
            {
                packet.WriteInteger(Colour.Id);
                packet.WriteString(Colour.Colour);
            }
        }
    }
}
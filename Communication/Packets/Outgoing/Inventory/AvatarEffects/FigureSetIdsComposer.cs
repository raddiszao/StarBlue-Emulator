using StarBlue.HabboHotel.Users.Clothing.Parts;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class FigureSetIdsComposer : MessageComposer
    {
        public ICollection<ClothingParts> ClothingParts { get; }

        public FigureSetIdsComposer(ICollection<ClothingParts> ClothingParts)
            : base(Composers.FigureSetIdsMessageComposer)
        {
            this.ClothingParts = ClothingParts;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
                packet.WriteInteger(Part.PartId);
            }

            packet.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
                packet.WriteString(Part.Part);
            }
        }
    }
}

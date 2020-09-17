
using StarBlue.HabboHotel.Users.Effects;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectExpiredComposer : MessageComposer
    {
        public AvatarEffect Effect { get; }

        public AvatarEffectExpiredComposer(AvatarEffect Effect)
            : base(Composers.AvatarEffectExpiredMessageComposer)
        {
            this.Effect = Effect;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Effect.SpriteId);
        }
    }
}

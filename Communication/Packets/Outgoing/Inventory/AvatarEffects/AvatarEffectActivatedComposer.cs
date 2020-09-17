
using StarBlue.HabboHotel.Users.Effects;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectActivatedComposer : MessageComposer
    {
        public AvatarEffect Effect { get; }

        public AvatarEffectActivatedComposer(AvatarEffect Effect)
            : base(Composers.AvatarEffectActivatedMessageComposer)
        {
            this.Effect = Effect;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Effect.SpriteId);
            packet.WriteInteger((int)Effect.Duration);
            packet.WriteBoolean(false);//Permanent
        }
    }
}
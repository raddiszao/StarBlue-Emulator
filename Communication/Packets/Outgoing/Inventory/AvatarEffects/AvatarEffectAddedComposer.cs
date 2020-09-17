namespace StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectAddedComposer : MessageComposer
    {
        public int SpriteId { get; }
        public int Duration { get; }

        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(Composers.AvatarEffectAddedMessageComposer)
        {
            this.SpriteId = SpriteId;
            this.Duration = Duration;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(SpriteId);
            packet.WriteInteger(0);//Types
            packet.WriteInteger(Duration);
            packet.WriteBoolean(false);//Permanent
        }
    }
}

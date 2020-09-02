namespace StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            base.WriteInteger(SpriteId);
            base.WriteInteger(0);//Types
            base.WriteInteger(Duration);
            base.WriteBoolean(false);//Permanent
        }
    }
}

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class AvatarEffectComposer : MessageComposer
    {
        private int playerID { get; }
        private int effectID { get; }

        public AvatarEffectComposer(int playerID, int effectID)
            : base(Composers.AvatarEffectMessageComposer)
        {
            this.playerID = playerID;
            this.effectID = effectID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(playerID);
            packet.WriteInteger(effectID);
            packet.WriteInteger(0);
        }
    }
}
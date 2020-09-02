namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int playerID, int effectID)
            : base(ServerPacketHeader.AvatarEffectMessageComposer)
        {
            base.WriteInteger(playerID);
            base.WriteInteger(effectID);
            base.WriteInteger(0);
        }
    }
}
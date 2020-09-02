namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class MysticBoxRewardComposer : ServerPacket
    {
        public MysticBoxRewardComposer(string type, int itemID)
            : base(ServerPacketHeader.MysticBoxRewardComposer)
        {
            base.WriteString(type);
            base.WriteInteger(itemID);
        }
    }
}
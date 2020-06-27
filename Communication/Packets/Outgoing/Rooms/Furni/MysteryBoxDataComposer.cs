using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class MysteryBoxDataComposer : ServerPacket
    {
        public MysteryBoxDataComposer(GameClient Session)
            : base(ServerPacketHeader.MysteryBoxDataComposer)
        {
            foreach (string box in Session.GetHabbo().MysticBoxes.ToArray())
            {
                base.WriteString(box);
            }
            foreach (string key in Session.GetHabbo().MysticKeys.ToArray())
            {
                base.WriteString(key);
            }
        }
    }
}

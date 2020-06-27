
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    class ModifyRoomFilterListEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Instance = Session.GetHabbo().CurrentRoom;
            if (Instance == null)
            {
                return;
            }

            if (!Instance.CheckRights(Session))
            {
                return;
            }

            int RoomId = Packet.PopInt();
            bool Added = Packet.PopBoolean();
            string Word = Packet.PopString();

            if (Word.Contains(":ban") || Word.Contains(":ha") || Word.Contains(":mute") || Word.Contains(":kick") || Word.Contains(":roomkick") || Word.Contains(":roommute"))
            {
                return;
            }

            if (Added)
            {
                Instance.GetFilter().AddFilter(Word);
            }
            else
            {
                Instance.GetFilter().RemoveFilter(Word);
            }
        }
    }
}

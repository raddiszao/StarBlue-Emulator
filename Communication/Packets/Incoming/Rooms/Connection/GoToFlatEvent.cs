using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Connection
{
    class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
            {
                Session.SendMessage(new CloseConnectionComposer());
            }
        }
    }
}

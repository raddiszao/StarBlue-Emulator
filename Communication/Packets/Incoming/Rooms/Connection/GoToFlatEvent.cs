using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Connection
{
    internal class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom || Session.GetHabbo().CurrentRoom == null)
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

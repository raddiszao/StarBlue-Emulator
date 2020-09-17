using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class InvinteHelperUserSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            IHelperElement Element = HelperToolsManager.GetElement(Session);
            HabboHotel.Rooms.Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            Element.OtherElement.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionInvinteRoomComposer(room.Id, room.RoomData.Name));
            Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionInvinteRoomComposer(room.Id, room.RoomData.Name));
        }
    }
}

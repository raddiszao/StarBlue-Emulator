using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session))
            {
                return;
            }

            string Name = Packet.PopString();
            bool Accepted = Packet.PopBoolean();

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Name);
            if (Client == null)
            {
                return;
            }

            if (Accepted)
            {
                Client.GetHabbo().RoomAuthOk = true;
                Client.SendMessage(new FlatAccessibleComposer(""));
                Room.SendMessage(new FlatAccessibleComposer(Client.GetHabbo().Username), true);
            }
            else
            {
                Client.SendMessage(new FlatAccessDeniedComposer(""));
                Room.SendMessage(new FlatAccessDeniedComposer(Client.GetHabbo().Username), true);
            }
        }
    }
}

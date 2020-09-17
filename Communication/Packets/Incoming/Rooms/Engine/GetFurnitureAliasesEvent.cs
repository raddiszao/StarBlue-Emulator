using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class GetFurnitureAliasesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new FurnitureAliasesComposer());
        }
    }
}

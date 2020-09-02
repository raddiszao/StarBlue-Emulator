
using StarBlue.Communication.Packets.Outgoing.Groups;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new BadgeEditorPartsComposer(
                StarBlueServer.GetGame().GetGroupManager().Bases,
                StarBlueServer.GetGame().GetGroupManager().Symbols,
                StarBlueServer.GetGame().GetGroupManager().BaseColours,
                StarBlueServer.GetGame().GetGroupManager().SymbolColours,
                StarBlueServer.GetGame().GetGroupManager().BackGroundColours));

        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Talents;
using StarBlue.HabboHotel.Achievements;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Talents
{
    class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            List<Talent> talents = StarBlueServer.GetGame().GetTalentManager().GetTalents(Type, -1);

            if (talents == null)
            {
                return;
            }

            Session.SendMessage(new TalentTrackComposer(Session, Type, talents));
        }
    }
}

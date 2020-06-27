﻿using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    class GetJukeboxPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
            {
                Session.SendMessage(new SetJukeboxPlayListComposer(Session.GetHabbo().CurrentRoom));
            }
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Navigator;
using System;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class LTDCountdownEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            string time = Packet.PopString();
            DateTime.TryParse(time, out DateTime date);
            TimeSpan diff = date - DateTime.Now;
            Session.SendMessage(new LTDCountdownComposer(time, diff));
        }
    }
}
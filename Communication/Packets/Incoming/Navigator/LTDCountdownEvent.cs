using StarBlue.Communication.Packets.Outgoing;
using System;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    class LTDCountdownEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string time = Packet.PopString();
            DateTime.TryParse(time, out DateTime date);
            TimeSpan diff = date - DateTime.Now;
            var response = new ServerPacket(ServerPacketHeader.LTDCountdownComposer);
            response.WriteString(time);
            response.WriteInteger(Convert.ToInt32(diff.TotalSeconds));
            Session.SendMessage(response);
        }
    }
}
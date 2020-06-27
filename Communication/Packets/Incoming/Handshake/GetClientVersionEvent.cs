using StarBlue.Core;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (StarBlueServer.SWFRevision != Build)
            {
                Logging.WriteLine(">> " + Build, ConsoleColor.DarkGray);
                StarBlueServer.SWFRevision = Build;
            }
        }
    }
}
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            Packet.PopBoolean();

            Habbo Data = StarBlueServer.GetHabboById(UserId);
            if (Data == null)
            {
                Session.SendNotification("Ocorreu um erro ao abrir o perfil deste usuário.");
                return;
            }

            Session.SendMessage(new ProfileInformationComposer(Data, Session));
        }
    }
}

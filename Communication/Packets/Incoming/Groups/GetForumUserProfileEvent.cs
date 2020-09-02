using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Groups.Forums
{
    internal class GetForumUserProfileEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Username = Packet.PopString();

            Habbo Data = StarBlueServer.GetHabboByUsername(Username);
            if (Data == null)
            {
                Session.SendNotification("Ocorreu um erro ao procurar o perfil deste usuário.");
                return;
            }

            Session.SendMessage(new ProfileInformationComposer(Data, Session));
        }
    }
}

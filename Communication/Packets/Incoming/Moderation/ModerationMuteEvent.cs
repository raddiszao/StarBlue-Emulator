using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_mute"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Length = (Packet.PopInt() * 60);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();

            Habbo Habbo = StarBlueServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro ao encontrar o usuário.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_mute") && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_any"))
            {
                Session.SendWhisper("Oops, não pode silenciar este usuário.");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `time_muted` = '" + Length + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TimeMuted = Length;
                Habbo.GetClient().SendNotification("Você foi silenciado por ter um mal comportamento no Hotel.");
                //Habbo.GetClient().SendNotification("usted ha sido silenciado por un moderador por  " + Length + " segundos!");
            }
        }
    }
}


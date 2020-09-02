using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Database.Interfaces;
using System.Collections.Generic;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class CheckValidNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            bool InUse = false;
            string Name = Packet.PopString();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `users` WHERE `username` = @name LIMIT 1");
                dbClient.AddParameter("name", Name);
                InUse = dbClient.GetInteger() == 1;
            }

            char[] Letters = Name.ToLower().ToCharArray();
            string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz.,_-;:?!1234567890";

            foreach (char Chr in Letters)
            {
                if (!AllowedCharacters.Contains(Chr))
                {
                    Session.SendMessage(new NameChangeUpdateComposer(Name, 4));
                    return;
                }
            }

            if (StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word))
            {
                Session.SendMessage(new NameChangeUpdateComposer(Name, 4));
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool") && Name.ToLower().Contains("mod") || Name.ToLower().Contains("adm") || Name.ToLower().Contains("admin") || Name.ToLower().Contains("m0d"))
            {
                Session.SendMessage(new NameChangeUpdateComposer(Name, 4));
                return;
            }

            else if (Name.Length > 15)
            {
                Session.SendMessage(new NameChangeUpdateComposer(Name, 3));
                return;
            }
            else if (Name.Length < 3)
            {
                Session.SendMessage(new NameChangeUpdateComposer(Name, 2));
                return;
            }
            else if (InUse)
            {
                ICollection<string> Suggestions = new List<string>();
                for (int i = 100; i < 103; i++)
                {
                    Suggestions.Add(i.ToString());
                }

                Session.SendMessage(new NameChangeUpdateComposer(Name, 5, Suggestions));
                return;
            }
            else
            {
                Session.SendMessage(new NameChangeUpdateComposer(Name, 0));
                return;
            }
        }
    }
}

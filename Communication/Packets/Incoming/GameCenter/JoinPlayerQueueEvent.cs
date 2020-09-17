using StarBlue.Communication.Packets.Outgoing.GameCenter;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Games;
using System;
using System.Data;
using System.Text;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    internal class JoinPlayerQueueEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if ((Session == null) || (Session.GetHabbo() == null))
            {
                return;
            }

            int GameId = Packet.PopInt();

            if (GameId == 1)
            {
                if (StarBlueServer.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData GameData))
                {
                    Session.SendMessage(new JoinQueueComposer(GameData.GameId));
                    int HabboID = Session.GetHabbo().Id;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        DataTable data;
                        dbClient.SetQuery("SELECT user_id FROM user_auth_ticket WHERE user_id = '" + HabboID + "'");
                        data = dbClient.GetTable();
                        int count = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            if (Convert.ToInt32(row["user_id"]) == HabboID)
                            {
                                count++;
                            }
                        }
                        if (count == 0)
                        {
                            string SSOTicket = "Fasfu-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;
                            dbClient.RunFastQuery("INSERT INTO user_auth_ticket(user_id, auth_ticket) VALUES ('" + HabboID +
                                              "', '" +
                                              SSOTicket + "')");
                            Session.SendMessage(new LoadGameComposer(SSOTicket));
                        }
                        else
                        {
                            dbClient.SetQuery("SELECT user_id,auth_ticket FROM user_auth_ticket WHERE user_id = " + HabboID);
                            data = dbClient.GetTable();
                            foreach (DataRow dRow in data.Rows)
                            {
                                object SSOTicket = dRow["auth_ticket"];
                                Session.SendMessage(new LoadGameComposer((string)SSOTicket));
                            }
                        }

                    }
                }
            }

            if (GameId == 2)
            {
                if (StarBlueServer.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData GameData))
                {
                    Session.SendMessage(new JoinQueueComposer(GameData.GameId));
                    int HabboID = Session.GetHabbo().Id;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        DataTable data;
                        dbClient.SetQuery("SELECT user_id FROM user_auth_ticket WHERE user_id = '" + HabboID + "'");
                        data = dbClient.GetTable();
                        int count = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            if (Convert.ToInt32(row["user_id"]) == HabboID)
                            {
                                count++;
                            }
                        }
                        if (count == 0)
                        {
                            string SSOTicket = "Snow-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;
                            dbClient.RunFastQuery("INSERT INTO user_auth_ticket(user_id, auth_ticket) VALUES ('" + HabboID +
                                              "', '" +
                                              SSOTicket + "')");
                            Session.SendMessage(new LoadGameComposer(SSOTicket));
                        }
                        else
                        {
                            dbClient.SetQuery("SELECT user_id,auth_ticket FROM user_auth_ticket WHERE user_id = " + HabboID);
                            data = dbClient.GetTable();
                            foreach (DataRow dRow in data.Rows)
                            {
                                object SSOTicket = dRow["auth_ticket"];
                                Session.SendMessage(new LoadGameComposer((string)SSOTicket));
                            }
                        }
                    }
                }
            }
        }

        private string GenerateSSO(int length)
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }

            return result.ToString();
        }
    }
}
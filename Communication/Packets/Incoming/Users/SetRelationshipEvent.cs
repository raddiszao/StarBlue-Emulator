using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.HabboHotel.Users.Relationships;
using System;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetRelationshipEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int User = Packet.PopInt();
            int Type = Packet.PopInt();

            if (!Session.GetHabbo().GetMessenger().FriendshipExists(User))
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oops, Solo puedes poner una relacion primeramente siendo amigos."));
                return;
            }

            if (Type < 0 || Type > 3)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oops, Ha elegido un tipo de relacion no valido."));
                return;
            }

            if (Session.GetHabbo().Relationships.Count > 2000)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Lo sentimos, el limite de relaciones es 2000"));
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                if (Type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.GetInteger();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(User))
                    {
                        Session.GetHabbo().Relationships.Remove(User);
                    }
                }
                else
                {
                    dbClient.SetQuery("SELECT id FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.GetInteger();

                    if (Id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", User);
                        dbClient.RunQuery();

                        if (Session.GetHabbo().Relationships.ContainsKey(User))
                        {
                            Session.GetHabbo().Relationships.Remove(User);
                        }
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + Session.GetHabbo().Id + "', @target, @type)");
                    dbClient.AddParameter("target", User);
                    dbClient.AddParameter("type", Type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());

                    if (!Session.GetHabbo().Relationships.ContainsKey(User))
                    {
                        Session.GetHabbo().Relationships.Add(User, new Relationship(newId, User, Type));
                    }
                }

                GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(User);
                if (Client != null)
                {
                    Session.GetHabbo().GetMessenger().UpdateFriend(User, Client, true);
                }
                else
                {
                    Habbo Habbo = StarBlueServer.GetHabboById(User);
                    if (Habbo != null)
                    {
                        if (Session.GetHabbo().GetMessenger().TryGetFriend(User, out MessengerBuddy Buddy))
                        {
                            Session.SendMessage(new FriendListUpdateComposer(Session, Buddy));
                        }
                    }
                }
            }
        }
    }
}
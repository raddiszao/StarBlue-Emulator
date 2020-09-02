using StarBlue.Communication.Packets.Outgoing.Inventory.Bots;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Bots
{
    internal class PlaceBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendNotification("Você não pode colocar bot aqui!");
                return;
            }

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false) || !Room.GetGameMap().ValidTile(X, Y))
            {
                Session.SendNotification("Você não pode colocar bot aqui!");
                return;
            }

            if (!Session.GetHabbo().GetInventoryComponent().TryGetBot(BotId, out Bot Bot))
            {
                return;
            }

            int BotCount = 0;
            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                {
                    continue;
                }

                BotCount += 1;
            }

            if (BotCount >= 5 && !Session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                Session.SendNotification("Sinto muito, é permitido somente 5 bots por sala!");
                return;
            }

            //TODO: Hmm, maybe not????
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = '" + Room.Id + "', `x` = @CoordX, `y` = @CoordY WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                dbClient.AddParameter("CoordX", X);
                dbClient.AddParameter("CoordY", Y);
                dbClient.RunQuery();
            }

            List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();

            //TODO: Grab data?
            DataRow GetData = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ai_type`,`rotation`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                GetData = dbClient.GetRow();

                dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @BotId");
                dbClient.AddParameter("BotId", Bot.Id);
                DataTable BotSpeech = dbClient.GetTable();

                foreach (DataRow Speech in BotSpeech.Rows)
                {
                    BotSpeechList.Add(new RandomSpeech(Convert.ToString(Speech["text"]), Bot.Id));
                }
            }

            RoomUser BotUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Session.GetHabbo().CurrentRoomId, Convert.ToString(GetData["ai_type"]), Convert.ToString(GetData["walk_mode"]), Bot.Name, "", Bot.Figure, X, Y, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, Bot.OwnerId, StarBlueServer.EnumToBool(GetData["automatic_chat"].ToString()), Convert.ToInt32(GetData["speaking_interval"]), StarBlueServer.EnumToBool(GetData["mix_sentences"].ToString()), Convert.ToInt32(GetData["chat_bubble"])), null);
            BotUser.Chat("Olá " + Session.GetHabbo().Username + "!", false, 0);
            if (GetData["ai_type"].Equals("visitor_logger"))
            {
                BotUser.Chat("Irei falar para você quem visitou seu quarto enquanto você esteve ausente.", false, 0);
            }

            BotUser.ApplyEffect(187);

            Room.GetGameMap().UpdateUserMovement(new System.Drawing.Point(X, Y), new System.Drawing.Point(X, Y), BotUser);

            if (!Session.GetHabbo().GetInventoryComponent().TryRemoveBot(BotId, out Bot ToRemove))
            {
                Console.WriteLine("Error whilst removing Bot: " + ToRemove.Id);
                return;
            }
            Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}

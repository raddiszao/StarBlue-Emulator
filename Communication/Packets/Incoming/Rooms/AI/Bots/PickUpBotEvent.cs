using StarBlue.Communication.Packets.Outgoing.Inventory.Bots;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using System;

namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Bots
{
    internal class PickUpBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int BotId = Packet.PopInt();
            if (BotId == 0)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser BotUser))
            {
                return;
            }

            if (Session.GetHabbo().Id != BotUser.BotData.ownerID && !Session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                Session.SendWhisper("Você só pode escolher os seus próprios Bot's!");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", BotId);
                dbClient.RunQuery();
            }


            Room.GetGameMap().RemoveUserFromMap(BotUser, new System.Drawing.Point(BotUser.X, BotUser.Y));

            Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
            Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
            Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
        }
    }
}

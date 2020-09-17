
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class ApplyDecorationEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());
            if (Item == null)
            {
                return;
            }

            if (Item.GetBaseItem() == null)
            {
                return;
            }

            string DecorationKey = string.Empty;
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.FLOOR:
                    DecorationKey = "floor";
                    break;

                case InteractionType.WALLPAPER:
                    DecorationKey = "wallpaper";
                    break;

                case InteractionType.LANDSCAPE:
                    DecorationKey = "landscape";
                    break;
            }

            switch (DecorationKey)
            {
                case "floor":
                    Room.RoomData.Floor = Item.ExtraData;

                    StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_FLOOR);
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFloor", 1);
                    break;

                case "wallpaper":
                    Room.RoomData.Wallpaper = Item.ExtraData;

                    StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_WALL);
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoWallpaper", 1);
                    break;

                case "landscape":
                    Room.RoomData.Landscape = Item.ExtraData;

                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoLandscape", 1);
                    break;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `rooms` SET " + DecorationKey + " = @extradata WHERE `id` = '" + Room.Id + "' LIMIT 1");
                dbClient.AddParameter("extradata", Item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + Item.Id + " LIMIT 1");
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
            Room.SendMessage(new RoomPropertyComposer(DecorationKey, Item.ExtraData));
        }
    }
}

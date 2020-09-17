using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    internal class AddStickyNoteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int itemId = Packet.PopInt();
            string locationData = Packet.PopString();

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session))
            {
                return;
            }

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(itemId);
            if (Item == null)
            {
                return;
            }

            string WallPossition = WallPositionCheck(":" + locationData.Split(':')[1]);

            Item RoomItem = new Item(Item.Id, Room.Id, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, Session.GetHabbo().Id, Item.GroupId, 0, 0, WallPossition, Room);
            if (Room.GetRoomItemHandler().SetWallItem(Session, RoomItem))
            {
                Session.GetHabbo().GetInventoryComponent().RemoveItem(itemId);
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_NotesLeft", 1);
            }
        }

        private static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                { return ":w=0,0 l=0,0 l"; }
                if (wallPosition.Contains(Convert.ToChar(9)))
                { return ":w=0,0 l=0,0 l"; }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return ":w=0,0 l=0,0 l";

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                //if (widthX < 0 || widthY < 0 || widthX > 200 || widthY > 200)
                //return ":w=0,0 l=0,0 l";

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                //if (lengthX < 0 || lengthY < 0 || lengthX > 200 || lengthY > 200)
                //return ":w=0,0 l=0,0 l";
                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return ":w=0,0 l=0,0 l";
            }
        }
    }
}

using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni
{
    internal class SetMannequinNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int ItemId = Packet.PopInt();
            string Name = Packet.PopString();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (Item.ExtraData.Contains(Convert.ToChar(5)))
            {
                string[] Flags = Item.ExtraData.Split(Convert.ToChar(5));
                Item.ExtraData = Flags[0] + Convert.ToChar(5) + Flags[1] + Convert.ToChar(5) + Name;
            }
            else
            {
                Item.ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `extra_data` = @Ed WHERE id = '" + Item.Id + "' LIMIT 1");
                dbClient.AddParameter("Ed", Item.ExtraData);
                dbClient.RunQuery();
            }

            Item.UpdateState(true, true);
        }
    }
}

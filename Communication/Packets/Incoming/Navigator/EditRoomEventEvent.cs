using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using System;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class EditRoomEventEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int RoomId = Packet.PopInt();
            string Name = Packet.PopString();
            Name = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word) ? "Spam" : Name;
            string Desc = Packet.PopString();
            Desc = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Data == null)
            {
                return;
            }

            if (Data.OwnerId != Session.GetHabbo().Id)
            {
                return; //HAX
            }

            if (Data.Promotion == null)
            {
                Session.SendNotification("Oops, não existe uma promoção nesta sala.");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_promotions` SET `title` = @title, `description` = @desc WHERE `room_id` = " + RoomId + " LIMIT 1");
                dbClient.AddParameter("title", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.RunQuery();
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(RoomId), out Room Room))
            {
                return;
            }

            Data.Promotion.Name = Name;
            Data.Promotion.Description = Desc;
            Room.SendMessage(new RoomEventComposer(Data, Data.Promotion));
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class BuyRoomCommand : IChatCommand
    {
        public string Description => "Compra um quarto à venda.";

        public string Parameters => "";

        public string PermissionRequired => "user_normal";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Room _Room = Session.GetHabbo().CurrentRoom;
            RoomUser RoomOwner = _Room.GetRoomUserManager().GetRoomUserByHabbo(_Room.RoomData.OwnerName);
            if (_Room == null)
            {
                return;
            }
            if (_Room.RoomData.OwnerName == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Você está tentando comprar um quarto que já é seu.", 34);
                return;
            }

            if (!Room.ForSale)
            {
                Session.SendWhisper("Este quarto não está à venda!", 34);
                return;
            }

            if (Session.GetHabbo().Duckets < _Room.SalePrice)
            {
                Session.SendWhisper("Você não tem Duckets suficientes para comprar esse quarto!", 34);
                return;
            }

            if (RoomOwner == null || RoomOwner.GetClient() == null)
            {
                Session.SendWhisper("Erro, o dono do quarto não se encontra.", 34);
                _Room.ForSale = false;
                _Room.SalePrice = 0;
                return;
            }

            GameClient Owner = RoomOwner.GetClient();

            Owner.GetHabbo().Duckets += _Room.SalePrice;
            Owner.SendMessage(new HabboActivityPointNotificationComposer(Owner.GetHabbo().Duckets, _Room.SalePrice));
            Session.GetHabbo().Duckets -= _Room.SalePrice;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, _Room.SalePrice));

            _Room.RoomData.OwnerName = Session.GetHabbo().Username;
            _Room.RoomData.OwnerId = Session.GetHabbo().Id;
            int RoomId = _Room.Id;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE rooms SET owner='" + Session.GetHabbo().Id + "' WHERE id='" + Room.Id + "' LIMIT 1");
                dbClient.RunFastQuery("UPDATE items SET user_id='" + Session.GetHabbo().Id + "' WHERE room_id='" + Room.Id + "'");
            }

            Session.GetHabbo().UsersRooms.Add(_Room.RoomData);
            Owner.GetHabbo().UsersRooms.Remove(_Room.RoomData);
            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(_Room.Id);

            Session.GetHabbo().PrepareRoom(RoomId, "");

        }
    }
}

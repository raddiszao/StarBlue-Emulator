using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class SetChessGameCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativa o modo xadrez."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Session.GetHabbo().PlayingChess)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Você não pode usar este comando se não tiver o modo xadrez ativado. Por favor, sente-se em uma cadeira de xadrez.", ""));
                return;
            }

            string CoordX = Params[1];
            string CoordY = Params[2];

            List<Item> IR = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
            foreach (Item _item in IR)
            {
                List<Item> IR2 = Room.GetGameMap().GetAllRoomItemForSquare(int.Parse(CoordX), int.Parse(CoordY));
                foreach (Item _item2 in IR2)
                {
                    Room.SendMessage(new SlideObjectBundleComposer(_item2.GetX, _item2.GetY, _item2.GetZ, 15, 12, 0, 0, 0, _item2.Id));
                    Room.GetRoomItemHandler().SetFloorItem(_item2, 15, 12, 0);
                }

                Room.SendMessage(new SlideObjectBundleComposer(_item.GetX, _item.GetY, _item.GetZ, int.Parse(CoordX), int.Parse(CoordY), 0, 0, 0, _item.Id));
                Room.GetRoomItemHandler().SetFloorItem(_item, int.Parse(CoordX), int.Parse(CoordY), 0);
                return;
            }
        }
    }
}
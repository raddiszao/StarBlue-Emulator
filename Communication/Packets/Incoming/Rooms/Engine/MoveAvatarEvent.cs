﻿using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveAvatarEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || !User.CanWalk)
            {
                return;
            }

            int MoveX = Packet.PopInt();
            int MoveY = Packet.PopInt();

            /*if (!User.CanWalk)
            {
                var OneGate = Room.GetGameMap().GetCoordinatedItems(User.Coordinate).Find(x => x.GetBaseItem().InteractionType == InteractionType.ONE_WAY_GATE);
                if (OneGate != null)
                {
                    if (MoveX == User.X - 1 && MoveY == User.Y + 1 && User.RotBody == 4 || MoveX == User.X + 1 && MoveY == User.Y + 1 && User.RotBody == 4 || MoveX == User.X - 1 && MoveY == User.Y + 1 && User.RotBody == 6 || MoveX == User.X - 1 && MoveY == User.Y - 1 && User.RotBody == 6 || MoveX == User.X + 1 && MoveY == User.Y - 1 && User.RotBody == 0 || MoveX == User.X - 1 && MoveY == User.Y - 1 && User.RotBody == 0 || MoveX == User.X + 1 && MoveY == User.Y + 1 && User.RotBody == 2 || MoveX == User.X + 1 && MoveY == User.Y - 1 && User.RotBody == 2)
                    {
                        User.NewX = MoveX;
                        User.NewY = MoveY;
                    }
                }
                return;
            }*/

            if (MoveX == User.X && MoveY == User.Y)
            {
                User.SeatCount++;

                if (User.SeatCount == 4)
                {
                    User.SeatCount = 0;
                    return;
                }
            }
            else
            {
                User.SeatCount = 0;
            }

            if (User.RidingHorse)
            {
                RoomUser Horse = Room.GetRoomUserManager().GetRoomUserByVirtualId(User.HorseID);
                if (Horse != null)
                {
                    Horse.MoveTo(MoveX, MoveY);
                }
            }

            if (Session.GetHabbo().isControlling)
            {
                RoomUser Controlled = Room.GetRoomUserManager().GetRoomUserByUsername(Session.GetHabbo().Opponent);
                if (Controlled == null)
                {
                    Session.GetHabbo().Opponent = "";
                    Session.GetHabbo().isControlling = false;
                }

                if (Controlled.CanWalk)
                {
                    Controlled.MoveTo(MoveX, MoveY);
                    return;
                }
            }

            User.boolcount = 0;
            User.MoveTo(MoveX, MoveY);
        }
    }
}
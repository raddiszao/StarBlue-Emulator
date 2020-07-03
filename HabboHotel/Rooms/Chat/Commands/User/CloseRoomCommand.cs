﻿namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class CloseRoomCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Fecha o quarto atual.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, só o dono do quarto pode usar este comando!", 34);
                return;
            }

            RoomAccess Access = RoomAccessUtility.ToRoomAccess(1);
            Room.Access = Access;
            Room.RoomData.Access = Access;
            Session.SendWhisper("O quarto foi trancado.", 34);
        }
    }
}

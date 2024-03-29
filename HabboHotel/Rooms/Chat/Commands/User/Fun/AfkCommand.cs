﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class AfkCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ficar ausente.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.IsAsleep = true;
            Room.SendMessage(new SleepComposer(User, true));
            User.ApplyEffect(517);

            Session.SendWhisper("Agora você está dormindo!", 34);
        }
    }
}

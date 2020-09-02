using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class InfoCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Informações do " + Convert.ToString(StarBlueServer.GetConfig().data["hotel.name"]) + ".";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - StarBlueServer.ServerStarted;

            Session.GetHabbo().SendWebPacket(new AboutServerComposer(StarBlueServer.GetGame().GetClientManager().Count, StarBlueServer.GetGame().GetRoomManager().Count, StarBlueServer.LastUpdate, Uptime.Days + " dia(s), " + Uptime.Hours + " hora(s) e " + Uptime.Minutes + " minuto(s)."));
        }
    }
}
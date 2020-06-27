using Newtonsoft.Json.Linq;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class InfoCommand : IChatCommand
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
            get { return "Informações do " + Convert.ToString(StarBlueServer.GetConfig().data["hotel.name"]) + "."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - StarBlueServer.ServerStarted;

            JObject WebEventData = new JObject(new JProperty("type", "aboutserver"), new JProperty("data", new JObject(
                new JProperty("users", StarBlueServer.GetGame().GetClientManager().Count),
                new JProperty("rooms", StarBlueServer.GetGame().GetRoomManager().Count),
                new JProperty("lastupdate", StarBlueServer.LastUpdate),
                new JProperty("wsclients", StarBlueServer.GetGame().GetWebEventManager()._webSockets.Count),
                new JProperty("uptime", Uptime.Days + " dia(s), " + Uptime.Hours + " hora(s) e " + Uptime.Minutes + " minuto(s).")
            )));
            StarBlueServer.GetGame().GetWebEventManager().SendDataDirect(Session, WebEventData.ToString());
        }
    }
}
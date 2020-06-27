using Newtonsoft.Json.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class YoutubeCommand : IChatCommand
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
            get { return "Coloca um vídeo do youtube no quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, somente o dono da sala pode usar este comando.", 34);
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve colocar um link de uma música do youtube.", 34);
                return;
            }

            string Link = Params[1];

            if (!Link.Contains("https://www.youtube.com/watch?v="))
            {
                Session.SendWhisper("Link do youtube inválido.", 34);
                return;
            }

            string YoutubeVideoId = Link.Split('=')[1].Split('&')[0];
            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (!YoutubeVideoId.Equals(""))
                {
                    JObject WebEventData = new JObject(new JProperty("type", "youtube"), new JProperty("data", new JObject(
                        new JProperty("video_id", YoutubeVideoId),
                        new JProperty("by", Session.GetHabbo().Username)
                    )));
                    StarBlueServer.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), WebEventData.ToString());
                }
                else
                {
                    Session.SendWhisper("Ocorreu um erro ao colocar o vídeo, tente novamente.");
                }
            }

        }
    }
}

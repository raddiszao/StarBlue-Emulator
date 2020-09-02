using StarBlue.Communication.Packets.Outgoing.WebSocket;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class YoutubeCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Coloca um vídeo do youtube no quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Oops, somente o dono da sala pode usar este comando.", 34);
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve colocar um link de uma música do youtube.", 34);
                return;
            }

            string Url = Params[1];

            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
            {
                return;
            }

            string Split = "";

            if (Url.Contains("?v="))
            {
                Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
            }
            else if (Url.Contains("youtu.be/"))
            {
                Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
            }

            if (Split.Length < 11)
            {
                return;
            }
            string YoutubeVideoId = Split.Substring(0, 11);

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (!YoutubeVideoId.Equals(""))
                {
                    User.GetClient().GetHabbo().SendWebPacket(new YoutubeVideoComposer(YoutubeVideoId, Session.GetHabbo().Username));
                }
                else
                {
                    Session.SendWhisper("Ocorreu um erro ao colocar o vídeo, tente novamente.");
                }
            }

        }
    }
}

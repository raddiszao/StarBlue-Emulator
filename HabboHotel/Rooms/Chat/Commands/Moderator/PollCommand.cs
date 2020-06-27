using StarBlue.HabboHotel.GameClients;
using System.Threading;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class PollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length <= 2)
            {
                Session.SendWhisper("Por favor coloque os valores válidos. [tempo, pergunta]");
            }
            else
            {
                if (!int.TryParse(Params[1], out int time))
                {
                    Session.SendWhisper("Valor do tempo não é um número inteiro.");
                    return;
                }

                string quest = CommandManager.MergeParams(Params, 2);
                if (quest == "end")
                {
                    Room.endQuestion();
                }
                else if (time != -1 || time != 0)
                {
                    Room.startQuestion(quest);
                    time = time * 864000;
                    Task t = Task.Factory.StartNew(() => TaskStopQuestion(Room, time));
                }
                else
                {
                    Room.startQuestion(quest);
                }
            }
        }

        public void TaskStopQuestion(Room room, int time)
        {
            Thread.Sleep(time);
            room.endQuestion();
        }

        public string Description =>
            "Realizar uma enquete rápida.";

        public string Parameters =>
            "[TEMPO] [PERGUNTA]";

        public string PermissionRequired =>
            "user_7";
    }
}
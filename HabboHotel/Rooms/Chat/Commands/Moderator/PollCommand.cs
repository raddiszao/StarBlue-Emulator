using StarBlue.HabboHotel.GameClients;
using StarBlue.Utilities;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class PollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Somente pessoas com direitos ou donos podem usar este comando.", 34);
                return;
            }

            if (Params.Length <= 2)
            {
                Session.SendWhisper("Por favor coloque os valores válidos. [tempo em minutos, pergunta]");
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
                else if (time > 0)
                {
                    Room.startQuestion(quest);

                    Threading threading = new Threading();
                    threading.SetMinutes(time);
                    threading.SetAction(() => Room.endQuestion());
                    threading.Start();
                }
                else
                {
                    Room.startQuestion(quest);
                }
            }
        }

        public string Description =>
            "Realizar uma enquete rápida.";

        public string Parameters =>
            "[TEMPO EM MINUTOS] [PERGUNTA]";

        public string PermissionRequired =>
            "user_normal";
    }
}
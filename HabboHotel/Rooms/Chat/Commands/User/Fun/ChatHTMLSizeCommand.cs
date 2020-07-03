using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class ChatHTMLSizeCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";
        public string Parameters => "Número do 1 ao 20. Para voltar ao normal coloque o numéro 12";
        public string Description => "Mudar o tamanho do seu nome";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, Você deve escrever um número de 1-20!", 34);
                return;
            }
            string chatColour = Params[1];

            bool isNumeric = int.TryParse(chatColour, out int chatsize);
            if (isNumeric)
            {
                if (Session.GetHabbo().chatHTMLColour == null || Session.GetHabbo().chatHTMLColour == String.Empty)
                {
                    Session.GetHabbo().chatHTMLColour = "000000";
                }
                switch (chatsize)
                {
                    case 12:
                        Session.GetHabbo().chatHTMLSize = 12;
                        Session.SendWhisper("O tamanho do seu nome voltou ao normal", 34);
                        break;
                    default:
                        bool isValid = true;
                        if (chatsize < 1)
                        {
                            isValid = false;
                        }

                        if (chatsize > 20)
                        {
                            isValid = false;
                        }
                        if (isValid)
                        {
                            Session.SendWhisper("Você mudou o tamanho pra " + chatsize, 34);
                            Session.GetHabbo().chatHTMLSize = chatsize;
                        }
                        else
                        {
                            Session.SendWhisper("Tamanho inválido, deve ser um número de 1-20.", 34);
                        }

                        break;
                }
            }
            else
            {
                Session.SendWhisper("Tamanho inválido, deve ser um número de 1-20.", 34);
            }
            return;
        }
    }
}

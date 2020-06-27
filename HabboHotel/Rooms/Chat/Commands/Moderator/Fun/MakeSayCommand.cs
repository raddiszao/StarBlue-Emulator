using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class MakeSayCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[USUARIO] [MENSAGEM]";
        public string Description => "Que outro usuário diga alguma coisa.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário corretamente");
            }
            else
            {
                string Message = CommandManager.MergeParams(Params, 2);
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                if (TargetUser != null)
                {
                    if (TargetUser.GetClient() != null && TargetUser.GetClient().GetHabbo() != null)
                    {
                        if (TargetUser.GetClient().GetHabbo().Username != "Raddis")
                        {
                            Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, TargetUser.LastBubble));
                        }
                        else
                        {
                            Session.SendWhisper("Esse usuário não pode dizer isso.", 34);
                        }
                    }
                }
                else
                {
                    Session.SendWhisper("O usuário não está no quarto.", 34);
                }
            }
        }
    }
}

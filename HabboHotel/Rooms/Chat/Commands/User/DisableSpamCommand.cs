using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class DisableSpamCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ativa ou desativa a opção de receber spam.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMessengerInvites = false;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowMessengerInvites ? "agora" : "agora não") + "recebe spams no console!", 34);
        }
    }
}

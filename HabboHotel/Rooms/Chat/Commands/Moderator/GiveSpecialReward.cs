using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class GiveSpecialReward : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[USUARIO]";
        public string Description => "";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor, digite um nome de usuário para recompensar.", 34);
                return;
            }

            GameClient Target = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, este usuário não foi encontrado!", 34);
                return;
            }

            Target.SendMessage(new NuxItemListComposer());
            Session.SendWhisper("Você enviou corretamente o prêmio especial para " + Target.GetHabbo().Username, 34);
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.WebSocket;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class BuilderToolCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ferramenta de construção.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Oops, somente pessoas com direitos podem usar esta ferramenta.", 34);
                return;
            }

            Session.GetHabbo().SendWebPacket(new BuilderToolComposer(false, Session.GetHabbo().StackHeight.ToString(), false, Session.GetHabbo().FurniRotation, false, Session.GetHabbo().FurniState));
        }
    }
}

using Newtonsoft.Json.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class BuilderToolCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ferramenta de construção.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, somente o dono do quarto pode usar a ferramenta.", 34);
                return;
            }

            JObject WebEventData = new JObject(new JProperty("type", "buildertool"), new JProperty("data", new JObject(
                new JProperty("stack", false),
                new JProperty("stackValue", Session.GetHabbo().StackHeight),
                new JProperty("rotation", false),
                new JProperty("rotationValue", Session.GetHabbo().FurniRotation),
                new JProperty("state", false),
                new JProperty("stateValue", Session.GetHabbo().FurniState)
            )));
            StarBlueServer.GetGame().GetWebEventManager().SendDataDirect(Session, WebEventData.ToString());
        }
    }
}

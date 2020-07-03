using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fan
{
    internal class BuildCommand : IChatCommand
    {
        public string PermissionRequired => "command_give";

        public string Parameters => "%height%";

        public string Description => "Habilita o teletransporte do quarto para construir com mais facilidade";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            string height = Params[1];
            if (Session.GetHabbo().Id == Room.OwnerId)
            {
                if (!Room.CheckRights(Session, true))
                {
                    return;
                }

                Item[] items = Room.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item Item in items.ToList())
                {
                    GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (Item.GetBaseItem().InteractionType == InteractionType.STACKTOOL)
                    {
                        Room.SendMessage(new UpdateMagicTileComposer(Item.Id, int.Parse(height)));
                    }
                }
            }
        }
    }
}
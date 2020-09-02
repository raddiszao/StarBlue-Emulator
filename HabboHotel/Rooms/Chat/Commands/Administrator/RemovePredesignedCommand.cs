using StarBlue.Database.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class RemovePredesignedCommand : IChatCommand
    {
        public string PermissionRequired => "user_17";
        public string Parameters => "";
        public string Description => "Remove o quarto da lista de quartos pré-designado";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
            {
                return;
            }
            //if (!StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().Exists((uint)Room.Id))
            //{
            //    Session.SendWhisper("O quarto não existe na lista.");
            //    return;
            //}

            uint predesignedId = 0U;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + ";");
                predesignedId = (uint)dbClient.GetInteger();

                dbClient.RunFastQuery("DELETE FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + " AND id = " +
                    predesignedId + ";");
            }

            StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.Remove(predesignedId);
            Session.SendWhisper("O quarto foi removido corretamente da lista.");
        }
    }
}
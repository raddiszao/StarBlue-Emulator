using StarBlue.Communication.Packets.Outgoing.Inventory.Pets;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.AI;
using System.Drawing;
using System.Linq;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class KickPetsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Expulsa todos os mascotes do quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Lamento, somente pessoas com direitos podem usar esse comando!", 34);
                return;
            }

            if (Room.GetRoomUserManager().GetPets().Count > 0)
            {
                foreach (RoomUser Pet in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (Pet == null)
                    {
                        continue;
                    }

                    if (Pet.RidingHorse)
                    {
                        RoomUser UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                        if (UserRiding != null)
                        {
                            UserRiding.RidingHorse = false;
                            UserRiding.ApplyEffect(-1);
                            UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                        }
                        else
                        {
                            Pet.RidingHorse = false;
                        }
                    }

                    Pet.PetData.RoomId = 0;
                    Pet.PetData.PlacedInRoom = false;

                    Pet pet = Pet.PetData;
                    if (pet != null)
                    {
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `bots` SET `room_id` = '0', `x` = '0', `Y` = '0', `Z` = '0' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                            dbClient.RunFastQuery("UPDATE `bots_petdata` SET `experience` = '" + pet.experience + "', `energy` = '" + pet.Energy + "', `nutrition` = '" + pet.Nutrition + "', `respect` = '" + pet.Respect + "' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                        }
                    }

                    if (pet.OwnerId != Session.GetHabbo().Id)
                    {
                        GameClient Target = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(pet.OwnerId);
                        if (Target != null)
                        {
                            Target.GetHabbo().GetInventoryComponent().TryAddPet(Pet.PetData);
                            Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);

                            Target.SendMessage(new PetInventoryComposer(Target.GetHabbo().GetInventoryComponent().GetPets()));
                            return;
                        }
                    }

                    Session.GetHabbo().GetInventoryComponent().TryAddPet(Pet.PetData);
                    Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);
                    Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
                }
                Session.SendWhisper("Feito, removido todos os mascotes.", 34);
            }
            else
            {
                Session.SendWhisper("Lamento, não há nenhum mascote aqui!", 34);
            }
        }
    }
}

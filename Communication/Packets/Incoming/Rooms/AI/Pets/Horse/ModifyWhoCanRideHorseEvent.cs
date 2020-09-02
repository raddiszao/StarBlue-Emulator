
using StarBlue.Communication.Packets.Outgoing.Rooms.AI.Pets;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    internal class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Room.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            if (Pet.PetData.AnyoneCanRide == 1)
            {
                Pet.PetData.AnyoneCanRide = 0;
            }
            else
            {
                Pet.PetData.AnyoneCanRide = 1;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `bots_petdata` SET `anyone_ride` = '" + Pet.PetData.AnyoneCanRide + "' WHERE `id` = '" + PetId + "' LIMIT 1");
            }

            Room.SendMessage(new PetInformationComposer(Pet.PetData));
        }
    }
}

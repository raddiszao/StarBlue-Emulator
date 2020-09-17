
using StarBlue.Communication.Packets.Outgoing.Rooms.AI.Pets;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;

namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets
{
    internal class GetPetCommandsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser _pet))
            {
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(PetId);
                if (User == null)
                {
                    return;
                }

                //Check some values first, please!
                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                {
                    return;
                }

                //And boom! Let us send the information composer 8-).
                Session.SendMessage(new PetInformationComposer(User.GetClient().GetHabbo()));
                return;
            }

            //Continue as a regular pet..
            if (_pet.RoomId != Session.GetHabbo().CurrentRoomId || _pet.PetData == null)
            {
                return;
            }

            Pet pet = _pet.PetData;
            Session.SendMessage(new PetTrainingPanelComposer(pet));
        }
    }
}

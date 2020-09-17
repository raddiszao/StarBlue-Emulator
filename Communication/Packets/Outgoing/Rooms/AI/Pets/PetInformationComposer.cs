using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class PetInformationComposer : MessageComposer
    {
        private Pet Pet { get; }
        private Habbo Habbo { get; }
        private bool isMounted { get; }

        public PetInformationComposer(Pet Pet, bool isMounted = false)
            : base(Composers.PetInformationMessageComposer)
        {
            this.Pet = Pet;
            this.isMounted = isMounted;
        }

        public override void Compose(Composer packet)
        {
            if (Habbo != null)
            {
                packet.WriteInteger(Habbo.Id);
                packet.WriteString(Habbo.Username);
                packet.WriteInteger(Habbo.Rank);
                packet.WriteInteger(10);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(Habbo.GetStats().Respect);
                packet.WriteInteger(Habbo.Id);
                packet.WriteInteger(Convert.ToInt32(Math.Floor((StarBlueServer.GetUnixTimestamp() - Habbo.AccountCreated) / 86400)));//How?
                packet.WriteString(Habbo.Username);
                packet.WriteInteger(1);//3 on hab
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);
                packet.WriteInteger(0);//5 on hab
                packet.WriteInteger(0); // Anyone can ride horse
                packet.WriteInteger(0);
                packet.WriteInteger(0);//512 on hab
                packet.WriteInteger(0);//1536
                packet.WriteInteger(0);//2560
                packet.WriteInteger(0);//3584
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(-1);//255 on hab
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteBoolean(false);
            }
            else
            {
                if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Pet.RoomId, out Room Room))
                {
                    return;
                }

                packet.WriteInteger(Pet.PetId);
                packet.WriteString(Pet.Name);
                packet.WriteInteger(Pet.Level);
                packet.WriteInteger(Pet.MaxLevel);
                packet.WriteInteger(Pet.experience);
                packet.WriteInteger(Pet.experienceGoal);
                packet.WriteInteger(Pet.Energy);
                packet.WriteInteger(Pet.MaxEnergy);
                packet.WriteInteger(Pet.Nutrition);
                packet.WriteInteger(Pet.MaxNutrition);
                packet.WriteInteger(Pet.Respect);
                packet.WriteInteger(Pet.OwnerId);
                packet.WriteInteger(Pet.Age);
                packet.WriteString(Pet.OwnerName);
                packet.WriteInteger(1);//3 on hab
                packet.WriteBoolean(Pet.Saddle > 0);
                packet.WriteBoolean(isMounted);
                packet.WriteInteger(0);//5 on hab
                packet.WriteInteger(Pet.AnyoneCanRide); // Anyone can ride horse
                packet.WriteInteger(0);
                packet.WriteInteger(0);//512 on hab
                packet.WriteInteger(0);//1536
                packet.WriteInteger(0);//2560
                packet.WriteInteger(0);//3584
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(-1);//255 on hab
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteBoolean(false);
            }
        }

        public PetInformationComposer(Habbo Habbo)
            : base(Composers.PetInformationMessageComposer)
        {

        }
    }
}

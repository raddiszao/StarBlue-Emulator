using StarBlue.HabboHotel.Rooms.AI;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class PetTrainingPanelComposer : MessageComposer
    {
        private Pet pet { get; }

        public PetTrainingPanelComposer(Pet pet)
            : base(Composers.PetTrainingPanelMessageComposer)
        {
            this.pet = pet;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(pet.PetId);//Pet Id for sure.

            //Commands available to be done.
            packet.WriteInteger(7);//Count
            {
                //packet.WriteInteger(pet.Type);//Breed?
                packet.WriteInteger(1);
                packet.WriteInteger(2);
                packet.WriteInteger(3);
                packet.WriteInteger(4);
                packet.WriteInteger(5);
                packet.WriteInteger(6);
                packet.WriteInteger(7);
            }

            //Commands that can be used NOW. (Level ups give you new commands etc).
            packet.WriteInteger(7);//Count
            {
                //packet.WriteInteger(pet.Type);//Breed?
                packet.WriteInteger(1);
                packet.WriteInteger(2);
                packet.WriteInteger(3);
                packet.WriteInteger(4);
                packet.WriteInteger(5);
                packet.WriteInteger(6);
                packet.WriteInteger(7);
            }
        }

        public int GetCount(int Level)
        {
            switch (Level)
            {
                case 1:
                case 2:
                    return 1;

                case 3:
                case 4:
                    return 2;

                case 5:
                case 6:
                    return 3;

                case 7:
                case 8:
                    return 4;

                case 9:
                case 10:
                    return 5;

                case 11:
                case 12:
                    return 6;

                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    return 8;

                default:
                    return 1;
            }
        }
    }
}


using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Pets
{
    internal class RespectPetNotificationMessageComposer : MessageComposer
    {
        private Pet Pet { get; }
        private Habbo Habbo { get; }
        private RoomUser User { get; }

        public RespectPetNotificationMessageComposer(Pet Pet)
            : base(Composers.RespectPetNotificationMessageComposer)
        {
            this.Pet = Pet;
        }

        public override void Compose(Composer packet)
        {
            if (Habbo != null)
            {
                packet.WriteInteger(User.VirtualId);
                packet.WriteInteger(User.VirtualId);
                packet.WriteInteger(Habbo.Id);//Pet Id, 100%
                packet.WriteString(Habbo.Username);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteString("FFFFFF");//Yeah..
                packet.WriteInteger(0);
                packet.WriteInteger(0);//Count - 3 ints.
                packet.WriteInteger(1);
            }
            else
            {
                packet.WriteInteger(Pet.VirtualId);
                packet.WriteInteger(Pet.VirtualId);
                packet.WriteInteger(Pet.PetId);//Pet Id, 100%
                packet.WriteString(Pet.Name);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteString(Pet.Color);
                packet.WriteInteger(0);
                packet.WriteInteger(0);//Count - 3 ints.
                packet.WriteInteger(1);
            }
        }

        public RespectPetNotificationMessageComposer(Habbo Habbo, RoomUser User)
            : base(Composers.RespectPetNotificationMessageComposer)
        {
            this.Habbo = Habbo;
            this.User = User;
        }
    }
}

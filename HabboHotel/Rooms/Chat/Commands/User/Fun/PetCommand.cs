using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class PetCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Transformar-se em um PET.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {


            RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
            {
                return;
            }

            if (!Room.RoomData.PetMorphsAllowed)
            {
                Session.SendWhisper("O dono da sala não permite que você se torne um PET.");
                if (Session.GetHabbo().PetId > 0)
                {
                    Session.SendWhisper("Vá você ainda tem uma metamorfosis, un-morphing.");
                    Session.GetHabbo().PetId = 0;

                    Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));
                    Room.SendMessage(new UsersComposer(RoomUser));
                }
                return;
            }

            if (Params.Length == 1 || Params[1].ToString().ToLower() == "list")
            {
                StringBuilder List = new StringBuilder("");
                List.AppendLine("                              - LISTA DE PETS -");
                List.AppendLine("Colocando estes parâmetros usando o comando: pet você pode se transformar em: ");
                List.AppendLine("  l[»]l 1) gato - Transforme-se em um gato.");
                List.AppendLine("  l[»]l 2) terrier - Transforme-se em um fox terrier.");
                List.AppendLine("  l[»]l 3) cocodrilo - Transforme-se em um cocodrilo.");
                List.AppendLine("  l[»]l 4) urso - Transforme-se em um urso.");
                List.AppendLine("  l[»]l 5) porco - Transforme-se em um porco.");
                List.AppendLine("  l[»]l 6) leão - Transforme-se em um leão.");
                List.AppendLine("  l[»]l 7) rinoceronte - Transforme-se em um rinoceronte.");
                List.AppendLine("  l[»]l 8) aranha - Transforme-se em uma aranha.");
                List.AppendLine("  l[»]l 9) tartaruga - Transforme-se em uma tartaruga.");
                List.AppendLine("  l[»]l 10) pintinho - Transforme-se em um pintinho.");
                List.AppendLine("  l[»]l 11) sapo - Transforme-se em uma sapo.");
                List.AppendLine("  l[»]l 14) macaco - Transforme-se em um macaco.");
                List.AppendLine("  l[»]l 15) cavalo - Transforme-se em um cavalo.");
                List.AppendLine("  l[»]l 16) demonio - Transforme-se em um demonio.");
                List.AppendLine("  l[»]l 17) coelho - Transforme-se em um coelho.");
                List.AppendLine("  l[»]l 21) pintinho - Transforme-se em um pintinho.");
                List.AppendLine("  l[»]l 23) macaco - Transfórmate em macaco.");
                List.AppendLine("  l[»]l 24) lobo - Transforme-se em um lobo.");
                List.AppendLine("  l[»]l 25) bebeterrier - Transforme-se em um bebeterrier.");
                List.AppendLine("  l[»]l 26) gnomo - Transforme-se em um gnomo.");
                List.AppendLine("  l[»]l 31) haloompa - Transforme-se em um haloompa.");
                List.AppendLine("  l[»]l 32) pedra - Transforme-se em uma pedra.");
                List.AppendLine("  l[»]l 33) velociraptor - Transforme-se em um velociraptor");
                List.AppendLine("  l[»]l 34) pterosaur - Transforme-se em um pterosaur.");
                List.AppendLine("  l[»]l 35) vaca - Transforme-se em uma vaca.");

                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            int TargetPetId = GetPetIdByString(Params[1].ToString());
            if (TargetPetId == 0)
            {
                Session.SendWhisper("Não existe mascote com este nome!");
                return;
            }

            //Change the users Pet Id.
            Session.GetHabbo().PetId = (TargetPetId == -1 ? 0 : TargetPetId);

            //Quickly remove the old user instance.
            Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendMessage(new UsersComposer(RoomUser));

            //Tell them a quick message.
            if (Session.GetHabbo().PetId > 0)
            {
                Session.SendWhisper("Use ':pet habbo' para voltar ao normal!");
            }
        }

        private int GetPetIdByString(string Pet)
        {
            switch (Pet.ToLower())
            {
                default:
                    return 0;
                case "habbo":
                    return -1;
                case "perro":
                    return 60;//This should be 0.
                case "gato":
                case "1":
                    return 1;
                case "terrier":
                case "2":
                    return 2;
                case "croc":
                case "croco":
                case "cocodrilo":
                case "3":
                    return 3;
                case "oso":
                case "4":
                    return 4;
                case "liz":
                case "cerdo":
                case "kill":
                case "5":
                    return 5;
                case "leon":
                case "rawr":
                case "6":
                    return 6;
                case "rhino":
                case "rinoceronte":
                case "7":
                    return 7;
                case "spider":
                case "arana":
                case "araña":
                case "8":
                    return 8;
                case "tortuga":
                case "9":
                    return 9;
                case "chick":
                case "chicken":
                case "pollo":
                case "10":
                    return 10;
                case "frog":
                case "rana":
                case "11":
                    return 11;
                case "monkey":
                case "mono":
                case "14":
                    return 14;
                case "horse":
                case "caballo":
                case "15":
                    return 15;
                case "bunny":
                case "conejo":
                case "17":
                    return 17;
                case "pigeon":
                case "pajaro":
                case "21":
                    return 21;
                case "demon":
                case "demonio":
                case "23":
                    return 23;
                case "babybear":
                case "bebeoso":
                case "24":
                    return 24;
                case "babyterrier":
                case "bebeterrier":
                case "25":
                    return 25;
                case "gnome":
                case "gnomo":
                case "26":
                    return 26;
                case "haloompa":
                case "31":
                    return 31;
                case "rock":
                case "piedra":
                case "32":
                    return 32;
                case "pterosaur":
                case "33":
                    return 33;
                case "velociraptor":
                case "34":
                    return 34;
                case "vaca":
                case "35":
                    return 35;
            }
        }
    }
}
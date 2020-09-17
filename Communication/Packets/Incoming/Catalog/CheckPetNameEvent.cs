using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class CheckPetNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            string PetName = Packet.PopString();
            if (PetName.Length < 2)
            {
                Session.SendMessage(new CheckPetNameComposer(2, "2"));
                return;
            }
            else if (PetName.Length > 15)
            {
                Session.SendMessage(new CheckPetNameComposer(1, "15"));
                return;
            }
            else if (!StarBlueServer.IsValidAlphaNumeric(PetName))
            {
                Session.SendMessage(new CheckPetNameComposer(3, ""));
                return;
            }
            else if (StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(PetName, out string word))
            {
                Session.SendMessage(new CheckPetNameComposer(4, "" + word));
                return;
            }

            Session.SendMessage(new CheckPetNameComposer(0, ""));
        }
    }
}
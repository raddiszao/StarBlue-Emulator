using StarBlue.Communication.Packets.Outgoing.Rooms.AI.Bots;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Bots
{
    internal class OpenBotActionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser BotUser))
            {
                return;
            }

            string BotSpeech = "";
            foreach (RandomSpeech Speech in BotUser.BotData.RandomSpeech.ToList())
            {
                BotSpeech += (Speech.Message + "\n");
            }

            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.AutomaticChat;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.SpeakingInterval;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.MixSentences;

            if (ActionId == 2 || ActionId == 5)
            {
                Session.SendMessage(new OpenBotActionComposer(BotUser, ActionId, BotSpeech));
            }
        }
    }
}
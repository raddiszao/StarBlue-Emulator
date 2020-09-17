
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.AI.Bots
{
    internal class OpenBotActionComposer : MessageComposer
    {
        private RoomUser BotUser { get; }
        private int ActionId { get; }
        private string BotSpeech { get; }

        public OpenBotActionComposer(RoomUser BotUser, int ActionId, string BotSpeech)
            : base(Composers.OpenBotActionMessageComposer)
        {
            this.BotUser = BotUser;
            this.ActionId = ActionId;
            this.BotSpeech = BotSpeech;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(BotUser.BotData.Id);
            packet.WriteInteger(ActionId);
            if (ActionId == 2)
            {
                packet.WriteString(BotSpeech);
            }
            else if (ActionId == 5)
            {
                packet.WriteString(BotUser.BotData.Name);
            }
        }
    }
}

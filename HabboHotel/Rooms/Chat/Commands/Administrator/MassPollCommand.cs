using StarBlue.Communication.Packets.Outgoing.Rooms.Polls;
using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class MassPollCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[ID]"; }
        }

        public string Description
        {
            get { return "Envia uma poll a todo o hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduza o ID da Poll que quer realizar.", 34);
                return;
            }

            if (StarBlueServer.GetGame().GetPollManager().TryGetPollForHotel(int.Parse(Params[1]), out RoomPoll poll))
            {
                if (poll.Type == RoomPollType.Poll)
                {
                    StarBlueServer.GetGame().GetClientManager().SendMessage(new PollOfferComposer(poll));
                }
            }
            return;
        }
    }
}

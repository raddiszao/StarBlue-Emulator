using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class CloseConnectionComposer : MessageComposer
    {
        private GameClient Session { get; }

        public CloseConnectionComposer(GameClient Session)
            : base(Composers.CloseConnectionMessageComposer)
        {
            this.Session = Session;
        }

        public CloseConnectionComposer()
            : base(Composers.CloseConnectionMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            if (Session != null)
            {
                Session.GetHabbo().IsTeleporting = false;
                Session.GetHabbo().TeleportingRoomID = 0;
                Session.GetHabbo().TeleporterId = 0;
                Session.GetHabbo().CurrentRoomId = 0;
            }
        }
    }
}

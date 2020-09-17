using StarBlue.HabboHotel.Games;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class GameListComposer : MessageComposer
    {
        private ICollection<GameData> Games { get; }

        public GameListComposer(ICollection<GameData> Games)
            : base(Composers.GameListMessageComposer)
        {
            this.Games = Games;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(StarBlueServer.GetGame().GetGameDataManager().GetCount());//Game count
            foreach (GameData Game in Games)
            {
                packet.WriteInteger(Game.GameId);
                packet.WriteString(Game.GameName);
                packet.WriteString(Game.ColourOne);
                packet.WriteString(Game.ColourTwo);
                packet.WriteString(Game.ResourcePath);
                packet.WriteString(Game.StringThree);
            }
        }
    }
}

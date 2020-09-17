using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class GetRoomFilterListComposer : MessageComposer
    {
        public List<string> WordFilterList { get; }
        public GetRoomFilterListComposer(List<string> WordFilterList)
            : base(Composers.GetRoomFilterListMessageComposer)
        {
            this.WordFilterList = WordFilterList;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(WordFilterList.Count);
            foreach (string Word in WordFilterList)
            {
                packet.WriteString(Word);
            }
        }
    }
}

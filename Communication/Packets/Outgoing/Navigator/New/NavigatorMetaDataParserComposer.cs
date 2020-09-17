using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorMetaDataParserComposer : MessageComposer
    {
        private ICollection<TopLevelItem> TopLevelItems { get; }

        public NavigatorMetaDataParserComposer(ICollection<TopLevelItem> TopLevelItems)
            : base(Composers.NavigatorMetaDataParserMessageComposer)
        {
            this.TopLevelItems = TopLevelItems;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(TopLevelItems.Count);//Count
            foreach (TopLevelItem TopLevelItem in TopLevelItems.ToList())
            {
                //TopLevelContext
                packet.WriteString(TopLevelItem.SearchCode);//Search code
                packet.WriteInteger(0);//Count of saved searches?
                /*{
                    //SavedSearch
                    packet.WriteInteger(TopLevelItem.Id);//Id
                   packet.WriteString(TopLevelItem.SearchCode);//Search code
                   packet.WriteString(TopLevelItem.Filter);//Filter
                   packet.WriteString(TopLevelItem.Localization);//localization
                }*/
            }
        }
    }
}

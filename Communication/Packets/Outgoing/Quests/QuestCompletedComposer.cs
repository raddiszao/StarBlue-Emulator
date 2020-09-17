
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;

namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedComposer : MessageComposer
    {
        private GameClient Session { get; }
        private Quest Quest { get; }


        public QuestCompletedComposer(GameClient Session, Quest Quest)
            : base(Composers.QuestCompletedMessageComposer)
        {
            this.Session = Session;
            this.Quest = Quest;
        }

        public override void Compose(Composer packet)
        {
            int AmountInCat = StarBlueServer.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int Number = Quest == null ? AmountInCat : Quest.Number;
            int UserProgress = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);

            packet.WriteString(Quest.Category);
            packet.WriteInteger(Number); // Quest progress in this cat
            packet.WriteInteger(AmountInCat); // Total quests in this cat
            packet.WriteInteger(Quest == null ? 3 : Quest.RewardType); // Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            packet.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            packet.WriteBoolean(Quest == null ? false : Session.GetHabbo().GetStats().QuestID == Quest.Id); // Quest started
            packet.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            packet.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            packet.WriteInteger(Quest == null ? 0 : Quest.Reward);
            packet.WriteString(Quest == null ? string.Empty : Quest.Name);
            packet.WriteInteger(UserProgress); // Current progress
            packet.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            packet.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            packet.WriteString("");
            packet.WriteString("");
            packet.WriteBoolean(true); // ?
            packet.WriteBoolean(true); // Activate next quest..
        }
    }
}

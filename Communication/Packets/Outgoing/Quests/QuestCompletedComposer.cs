﻿
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;

namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedComposer : ServerPacket
    {
        public QuestCompletedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {
            int AmountInCat = StarBlueServer.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int Number = Quest == null ? AmountInCat : Quest.Number;
            int UserProgress = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);

            base.WriteString(Quest.Category);
            base.WriteInteger(Number); // Quest progress in this cat
            base.WriteInteger(AmountInCat); // Total quests in this cat
            base.WriteInteger(Quest == null ? 3 : Quest.RewardType); // Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            base.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            base.WriteBoolean(Quest == null ? false : Session.GetHabbo().GetStats().QuestID == Quest.Id); // Quest started
            base.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            base.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            base.WriteInteger(Quest == null ? 0 : Quest.Reward);
            base.WriteString(Quest == null ? string.Empty : Quest.Name);
            base.WriteInteger(UserProgress); // Current progress
            base.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            base.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            base.WriteString("");
            base.WriteString("");
            base.WriteBoolean(true); // ?
            base.WriteBoolean(true); // Activate next quest..
        }
    }
}

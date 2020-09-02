﻿using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    public class QuestListComposer : ServerPacket
    {
        public QuestListComposer(GameClient Session, List<Quest> Quests, bool Send, Dictionary<string, int> UserQuestGoals, Dictionary<string, Quest> UserQuests)
            : base(ServerPacketHeader.QuestListMessageComposer)
        {
            base.WriteInteger(UserQuests.Count);

            // Active ones first
            foreach (KeyValuePair<string, Quest> UserQuest in UserQuests)
            {
                if (UserQuest.Value == null)
                {
                    continue;
                }

                SerializeQuest(this, Session, UserQuest.Value, UserQuest.Key);
            }

            // Dead ones last
            foreach (KeyValuePair<string, Quest> UserQuest in UserQuests)
            {
                if (UserQuest.Value != null)
                {
                    continue;
                }

                SerializeQuest(this, Session, UserQuest.Value, UserQuest.Key);
            }

            base.WriteBoolean(Send);
        }

        private void SerializeQuest(ServerPacket Message, GameClient Session, Quest Quest, string Category)
        {
            if (Message == null || Session == null)
            {
                return;
            }

            int AmountInCat = StarBlueServer.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Category);
            int Number = Quest == null ? AmountInCat : Quest.Number - 1;
            int UserProgress = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);

            if (Quest != null && Quest.IsCompleted(UserProgress))
            {
                Number++;
            }

            Message.WriteString(Category);
            Message.WriteInteger(Number);  // Quest progress in this cat
            Message.WriteInteger(AmountInCat); // Total quests in this cat
            Message.WriteInteger(Quest == null ? 3 : Quest.RewardType);// Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            Message.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            Message.WriteBoolean(Quest == null ? false : Session.GetHabbo().GetStats().QuestID == Quest.Id);  // Quest started
            Message.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            Message.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            Message.WriteInteger(Quest == null ? 0 : Quest.Reward);
            Message.WriteString(Quest == null ? string.Empty : Quest.Name);
            Message.WriteInteger(UserProgress); // Current progress
            Message.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            Message.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            Message.WriteString("");
            Message.WriteString("");
            Message.WriteBoolean(true);
        }
    }
}
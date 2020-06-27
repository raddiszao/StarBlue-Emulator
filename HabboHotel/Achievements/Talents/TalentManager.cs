﻿using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Talents;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Achievements
{
    /// <summary>
    ///     Class TalentManager.
    /// </summary>
    public class TalentManager
    {
        /// <summary>
        ///     The talents
        /// </summary>
        public Dictionary<int, Talent> Talents = new Dictionary<int, Talent>();

        /// <summary>
        ///     Initializes the specified database client.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        public void Initialize()
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM talents_data ORDER BY `order_num` ASC");

                DataTable table = dbClient.GetTable();

                foreach (Talent talent in from DataRow dataRow in table.Rows
                                          select new Talent(
                                              (int)dataRow["id"],
                                              (string)dataRow["type"],
                                              (int)dataRow["parent_category"],
                                              (int)dataRow["level"],
                                              (string)dataRow["achievement_group"],
                                              (int)dataRow["achievement_level"],
                                              (string)dataRow["prize"],
                                              (int)dataRow["prize_baseitem"]))
                {
                    Talents.Add(talent.Id, talent);
                }
            }
        }

        /// <summary>
        ///     Gets the talent.
        /// </summary>
        /// <param name="talentId">The talent identifier.</param>
        /// <returns>Talent.</returns>
        public Talent GetTalent(int talentId) => Talents[talentId];

        /// <summary>
        ///     Levels the is completed.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="trackType">Type of the track.</param>
        /// <param name="talentLevel">The talent level.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool LevelIsCompleted(GameClient session, string trackType, int talentLevel) =>
               GetTalents(trackType, talentLevel).All(
                   current =>
                       !session.GetHabbo().Achievements.ContainsKey(current.AchievementGroup) ||
                       session.GetHabbo().GetAchievementData(current.AchievementGroup).Level < current.AchievementLevel);

        /// <summary>
        ///     Completes the user talent.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="talent">The talent.</param>
        public void CompleteUserTalent(GameClient session, Talent talent)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().CurrentTalentLevel < talent.Level ||
                session.GetHabbo().Talents.ContainsKey(talent.Id))
            {
                return;
            }

            if (!LevelIsCompleted(session, talent.Type, talent.Level))
            {
                return;
            }

            if (!string.IsNullOrEmpty(talent.Prize) && talent.PrizeBaseItem > 0)
            {
                if (!StarBlueServer.GetGame().GetItemManager().GetItem(talent.PrizeBaseItem, out ItemData item))
                {
                    return;
                }

                Item GiveItem = ItemFactory.CreateSingleItemNullable(item, session.GetHabbo(), string.Empty, string.Empty);
                if (GiveItem != null)
                {
                    session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));

                    session.SendMessage(new PurchaseOKComposer());
                    session.SendMessage(new FurniListAddComposer(GiveItem));
                    session.SendMessage(new FurniListUpdateComposer());
                }
            }

            session.GetHabbo().Talents.Add(talent.Id, new UserTalent(talent.Id, 1));

            using (IQueryAdapter queryReactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Concat("REPLACE INTO users_talents VALUES (", session.GetHabbo().Id, ", ", talent.Id, ", ", 1, ");"));
            }

            session.SendMessage(new TalentLevelUpComposer(talent));

            if (talent.Type == "citizenship")
            {
                switch (talent.Level)
                {
                    case 3:
                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_Citizenship", 1);
                        break;
                    case 4:
                        // session.GetHabbo().GetSubscriptionManager().AddSubscription(7);

                        using (IQueryAdapter queryReactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            queryReactor.RunFastQuery(string.Concat(new object[] { "UPDATE users SET talent_status = 'helper' WHERE id = ", session.GetHabbo().Id, ";" }));
                        }
                        break;
                }
            }
        }

        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetTalent(string AchGroup, out Talent talent)
        {
            foreach (Talent current in Talents.Values)
            {
                if (current.AchievementGroup == AchGroup)
                {
                    talent = current;
                    return true;
                }
            }
            talent = null;
            return false;
        }

        /// <summary>
        ///     Tries the get talent.
        /// </summary>
        /// <param name="achGroup">The ach group.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public Talent GetTalentData(string achGroup)
               => Talents.Values.FirstOrDefault(current => current.AchievementGroup == achGroup);

        /// <summary>
        ///     Gets all talents.
        /// </summary>
        /// <returns>Dictionary&lt;System.Int32, Talent&gt;.</returns>
        public Dictionary<int, Talent> GetAllTalents() => Talents;

        /// <summary>
        ///     Gets the talents.
        /// </summary>
        /// <param name="trackType">Type of the track.</param>
        /// <param name="parentCategory">The parent category.</param>
        /// <returns>List&lt;Talent&gt;.</returns>
        public List<Talent> GetTalents(string trackType, int parentCategory)
        {
            return Talents.Values.Where(current => current.Type == trackType && current.ParentCategory == parentCategory).ToList();
        }
    }
}
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Items
{
    internal class CrackableItem
    {
        internal uint ItemId;
        internal List<CrackableRewards> Rewards;

        internal CrackableItem(DataRow dRow)
        {
            ItemId = Convert.ToUInt32(dRow["item_baseid"]);
            string rewardsString = (string)dRow["rewards"];

            Rewards = new List<CrackableRewards>();
            foreach (string reward in rewardsString.Split(';'))
            {
                string rewardType = reward.Split(',')[0];
                string rewardItem = reward.Split(',')[1];
                uint rewardLevel = uint.Parse(reward.Split(',')[2]);
                Rewards.Add(new CrackableRewards(ItemId, rewardType, rewardItem, rewardLevel));
            }
        }
    }

    internal class CrackableRewards
    {
        internal uint CrackableId, CrackableLevel;
        internal string CrackableRewardType, CrackableReward;

        internal CrackableRewards(uint crackableId, string crackableRewardType, string crackableReward, uint crackableLevel)
        {
            CrackableId = crackableId;
            CrackableRewardType = crackableRewardType;
            CrackableReward = crackableReward;
            CrackableLevel = crackableLevel;
        }
    }

    internal class CrackableManager
    {
        internal Dictionary<int, CrackableItem> Crackable;

        internal void Initialize(IQueryAdapter dbClient)
        {
            Crackable = new Dictionary<int, CrackableItem>();
            dbClient.SetQuery("SELECT * FROM crackable_rewards");
            DataTable table = dbClient.GetTable();
            foreach (DataRow dRow in table.Rows)
            {
                if (Crackable.ContainsKey(Convert.ToInt32(dRow["item_baseid"])))
                {
                    continue;
                }

                Crackable.Add(Convert.ToInt32(dRow["item_baseid"]), new CrackableItem(dRow));
            }
        }


        private List<CrackableRewards> GetRewardsByLevel(int itemId, int level)
        {
            List<CrackableRewards> rewards = new List<CrackableRewards>();
            foreach (CrackableRewards reward in Crackable[itemId].Rewards.Where(furni => furni.CrackableLevel == level))
            {
                rewards.Add(reward);
            }

            return rewards;
        }

        internal void ReceiveCrackableReward(RoomUser user, Room room, Item item)
        {

            if (room == null || item == null)
            {
                return;
            }

            if (item.GetBaseItem().InteractionType != InteractionType.PINATA && item.GetBaseItem().InteractionType != InteractionType.PINATATRIGGERED && item.GetBaseItem().InteractionType != InteractionType.MAGICEGG)
            {
                return;
            }

            if (!Crackable.ContainsKey(item.GetBaseItem().Id))
            {
                return;
            }

            Crackable.TryGetValue(item.GetBaseItem().Id, out CrackableItem crackable);
            if (crackable == null)
            {
                return;
            }

            int x = item.GetX, y = item.GetY;
            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
            int level = 0;
            int rand = new Random().Next(0, 100);
            if (rand >= 95)
            {
                level = 5;                   // 005% de probabilidad de que salga nivel 5
            }
            else if (rand >= 85 && rand < 95)
            {
                level = 4; // 010% de probabilidad de que salga nivel 4
            }
            else if (rand >= 65 && rand < 85)
            {
                level = 3; // 020% de probabilidad de que salga nivel 3
            }
            else if (rand >= 35 && rand < 65)
            {
                level = 2; // 030% de probabilidad de que salga nivel 2
            }
            else
            {
                level = 1;                              // 035% de probabilidad de que salga nivel 1
            }
            // 100%

            List<CrackableRewards> possibleRewards = GetRewardsByLevel((int)crackable.ItemId, level);
            CrackableRewards reward = possibleRewards[new Random().Next(0, (possibleRewards.Count - 1))];

            Task.Run(() =>
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {

                    #region REWARD TYPES
                    switch (reward.CrackableRewardType)
                    {

                        #region NORMAL ITEMS REWARD
                        case "item":
                            goto ItemType;
                        #endregion

                        #region CREDITS REWARD
                        case "credits":
                            {
                                user.GetClient().GetHabbo().Credits += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new CreditBalanceComposer(user.GetClient().GetHabbo().Credits));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("cred", "Acaba de ganhar" + int.Parse(reward.CrackableReward) + " créditos na pinhata.", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region DUCKETS REWARD
                        case "duckets":
                            {
                                user.GetClient().GetHabbo().Duckets += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Duckets, user.GetClient().GetHabbo().Duckets));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("duckets", "Acaba de ganhar" + int.Parse(reward.CrackableReward) + " duckets na pinhata..", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region DIAMONDS REWARD
                        case "diamonds":
                            {
                                user.GetClient().GetHabbo().Diamonds += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Diamonds, 0, 5));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("diamonds", "Acaba de ganhar" + int.Parse(reward.CrackableReward) + " diamantes na pinhata..", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region HONOR REWARD
                        case "honors":
                            {
                                user.GetClient().GetHabbo().GOTWPoints += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().GOTWPoints, 0, 103));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("honor", "Acaba de ganhar " + int.Parse(reward.CrackableReward) + " " + StarBlueServer.GetSettingsManager().TryGetValue("seasonal.currency.name") + " na pinhata..", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region BADGE REWARD
                        case "badge":
                            {
                                if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(reward.CrackableReward))
                                {
                                    return;
                                }

                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar o emblema: " + reward.CrackableReward + ".", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));

                                user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(reward.CrackableReward, true, user.GetClient());
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.RunFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                            #endregion
                    }
                    #endregion

                    ItemType:
                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                    dbClient.RunFastQuery("UPDATE items SET base_item = " + int.Parse(reward.CrackableReward) + ", extra_data = '' WHERE id = " + item.Id);
                    item.BaseItem = int.Parse(reward.CrackableReward);
                    item.ResetBaseItem();
                    item.ExtraData = string.Empty;
                    if (!room.GetRoomItemHandler().SetFloorItem(user.GetClient(), item, item.GetX, item.GetY, item.Rotation, true, false, true))
                    {
                        dbClient.RunFastQuery("UPDATE items SET room_id = 0 WHERE id = " + item.Id);
                        user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
                    }
                }

                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acaba de ganhar " + rand + " nos dados.", ""));
            });
        }

    }
}
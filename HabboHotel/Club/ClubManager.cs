using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users.UserDataManagement;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Club
{
    internal class ClubManager
    {
        private readonly int UserId;
        private readonly Dictionary<string, Subscription> Subscriptions;

        internal ClubManager(int userID, UserData userData)
        {
            UserId = userID;
            Subscriptions = userData.subscriptions;
        }

        internal void Clear()
        {
            Subscriptions.Clear();
        }

        internal Subscription GetSubscription(string SubscriptionId)
        {
            if (Subscriptions.ContainsKey(SubscriptionId))
            {
                return Subscriptions[SubscriptionId];
            }
            else
            {
                return null;
            }
        }

        internal bool HasSubscription(string SubscriptionId)
        {
            if (!Subscriptions.ContainsKey(SubscriptionId))
            {
                return false;
            }

            Subscription subscription = Subscriptions[SubscriptionId];
            return subscription.IsValid();
        }

        internal void AddOrExtendSubscription(string SubscriptionId, int DurationSeconds, GameClient Session)
        {
            SubscriptionId = SubscriptionId.ToLower();


            if (Subscriptions.ContainsKey(SubscriptionId))
            {
                Subscription subscription = Subscriptions[SubscriptionId];

                if (subscription.IsValid())
                {
                    subscription.ExtendSubscription(DurationSeconds);
                }
                else
                {
                    subscription.SetEndTime((int)StarBlueServer.GetUnixTimestamp() + DurationSeconds);
                }

                //using (IQueryAdapter adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //  {
                //      adapter.SetQuery(string.Concat(new object[] { "UPDATE user_subscriptions SET timestamp_expire = ", subscription.ExpireTime, " WHERE user_id = ", this.UserId, " AND subscription_id = '", subscription.SubscriptionId, "'" }));
                //       adapter.RunQuery();
                //   }
            }
            else
            {
                int unixTimestamp = (int)StarBlueServer.GetUnixTimestamp();
                int timeExpire = (int)StarBlueServer.GetUnixTimestamp() + DurationSeconds;
                string SubscriptionType = SubscriptionId;
                Subscription subscription2 = new Subscription(SubscriptionId, timeExpire);

                using (IQueryAdapter adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.RunFastQuery(string.Concat(new object[] { "INSERT INTO user_subscriptions (user_id,subscription_id,timestamp_activated,timestamp_expire) VALUES (", UserId, ",'", SubscriptionType, "',", unixTimestamp, ",", timeExpire, ")" }));
                }

                Subscriptions.Add(subscription2.SubscriptionId.ToLower(), subscription2);
            }
        }

        internal void ReloadSubscription(GameClient Session)
        {
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));
        }
    }
}
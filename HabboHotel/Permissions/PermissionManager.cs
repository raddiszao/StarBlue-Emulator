﻿using log4net;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Permissions
{
    public sealed class PermissionManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Permissions.PermissionManager");

        private readonly Dictionary<int, Permission> Permissions = new Dictionary<int, Permission>();

        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();

        private readonly Dictionary<int, List<string>> PermissionGroupRights = new Dictionary<int, List<string>>();

        private readonly Dictionary<int, List<string>> PermissionSubscriptionRights = new Dictionary<int, List<string>>();

        public PermissionManager()
        {

        }

        public void Init()
        {
            Permissions.Clear();
            _commands.Clear();
            PermissionGroupRights.Clear();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions`");
                DataTable GetPermissions = dbClient.GetTable();

                if (GetPermissions != null)
                {
                    foreach (DataRow Row in GetPermissions.Rows)
                    {
                        Permissions.Add(Convert.ToInt32(Row["id"]), new Permission(Convert.ToInt32(Row["id"]), Convert.ToString(Row["permission"]), Convert.ToString(Row["description"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_commands`");
                DataTable GetCommands = dbClient.GetTable();

                if (GetCommands != null)
                {
                    foreach (DataRow Row in GetCommands.Rows)
                    {
                        _commands.Add(Convert.ToString(Row["command"]), new PermissionCommand(Convert.ToString(Row["command"]), Convert.ToInt32(Row["group_id"]), Convert.ToInt32(Row["subscription_id"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_rights`");
                DataTable GetPermissionRights = dbClient.GetTable();

                if (GetPermissionRights != null)
                {
                    foreach (DataRow Row in GetPermissionRights.Rows)
                    {
                        int GroupId = Convert.ToInt32(Row["group_id"]);
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);

                        if (!Permissions.TryGetValue(PermissionId, out Permission Permission))
                        {
                            continue; // permission does not exist
                        }

                        if (PermissionGroupRights.ContainsKey(GroupId))
                        {
                            PermissionGroupRights[GroupId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            PermissionGroupRights.Add(GroupId, RightsSet);
                        }

                    }
                }
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_subscriptions`");
                DataTable GetPermissionSubscriptions = dbClient.GetTable();

                if (GetPermissionSubscriptions != null)
                {
                    foreach (DataRow Row in GetPermissionSubscriptions.Rows)
                    {
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);
                        int SubscriptionId = Convert.ToInt32(Row["subscription_id"]);

                        if (!Permissions.TryGetValue(PermissionId, out Permission Permission))
                        {
                            continue; // permission does not exist
                        }

                        if (PermissionSubscriptionRights.ContainsKey(SubscriptionId))
                        {
                            PermissionSubscriptionRights[SubscriptionId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            PermissionSubscriptionRights.Add(SubscriptionId, RightsSet);
                        }
                    }
                }
            }

            /*log.Info(">> Permissions " + this.Permissions.Count + " loaded.");
            log.Info(">> Permissions Groups " + this.PermissionGroups.Count + " loaded");
            log.Info(">> Permissions Rights " + this.PermissionGroupRights.Count + " loaded");
            log.Info(">> Permissions Subscription " + this.PermissionSubscriptionRights.Count + " loaded");*/
            log.Info(">> Permissions Manager -> READY!");
        }


        public List<string> GetPermissionsForPlayer(Habbo Player)
        {
            List<string> PermissionSet = new List<string>();

            if (PermissionGroupRights.TryGetValue(Player.Rank, out List<string> PermRights))
            {
                PermissionSet.AddRange(PermRights);
            }

            if (PermissionSubscriptionRights.TryGetValue(Player.VIPRank, out List<string> SubscriptionRights))
            {
                PermissionSet.AddRange(SubscriptionRights);
            }

            return PermissionSet;
        }

        public List<string> GetCommandsForPlayer(Habbo Player)
        {
            return _commands.Where(x => Player.Rank >= x.Value.GroupId && Player.VIPRank >= x.Value.SubscriptionId).Select(x => x.Key).ToList();
        }

        public List<string> GetCommandsForID(int id)
        {
            return _commands.Where(x => id == x.Value.GroupId).Select(x => x.Key).ToList();
        }
    }
}
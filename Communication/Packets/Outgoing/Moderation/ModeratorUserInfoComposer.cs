﻿using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserInfoComposer : MessageComposer
    {
        private DataRow User { get; }
        private DataRow Info { get; }

        public ModeratorUserInfoComposer(DataRow User, DataRow Info)
            : base(Composers.ModeratorUserInfoMessageComposer)
        {
            this.User = User;
            this.Info = Info;
        }

        public override void Compose(Composer packet)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(Info["trading_locked"]));


            packet.WriteInteger(User != null ? Convert.ToInt32(User["id"]) : 0);
            packet.WriteString(User != null ? Convert.ToString(User["username"]) : "Unknown");
            packet.WriteString(User != null ? Convert.ToString(User["look"]) : "Unknown");
            packet.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((StarBlueServer.GetUnixTimestamp() - Convert.ToDouble(User["account_created"])) / 60)) : 0);
            packet.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((StarBlueServer.GetUnixTimestamp() - Convert.ToDouble(User["last_online"])) / 60)) : 0);
            packet.WriteBoolean(User != null ? StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(User["id"])) != null : false);
            packet.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs"]) : 0);
            packet.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs_abusive"]) : 0);
            packet.WriteInteger(Info != null ? Convert.ToInt32(Info["cautions"]) : 0);
            packet.WriteInteger(Info != null ? Convert.ToInt32(Info["bans"]) : 0);
            packet.WriteInteger(Info != null ? Convert.ToInt32(Info["trading_locks_count"]) : 0);//Trading lock counts
            packet.WriteString(Convert.ToDouble(Info["trading_locked"]) != 0 ? (origin.ToString("dd/MM/yyyy HH:mm:ss")) : "0");//Trading lock
            packet.WriteString("");//Purchases
            packet.WriteInteger(0);//Itendity information tool
            packet.WriteInteger(0);//Id bans.
            packet.WriteString(User != null ? Convert.ToString(User["mail"]) : "Unknown");
            packet.WriteString("");//user_classification
        }
    }
}

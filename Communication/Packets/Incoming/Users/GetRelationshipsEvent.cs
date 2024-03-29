﻿using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.Users;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Habbo Habbo = StarBlueServer.GetHabboById(Packet.PopInt());
            if (Habbo == null)
            {
                return;
            }

            Random rand = new Random();
            Habbo.Relationships = Habbo.Relationships.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);

            int Loves = Habbo.Relationships.Count(x => x.Value.Type == 1);
            int Likes = Habbo.Relationships.Count(x => x.Value.Type == 2);
            int Hates = Habbo.Relationships.Count(x => x.Value.Type == 3);

            Session.SendMessage(new GetRelationshipsComposer(Habbo, Loves, Likes, Hates));
        }
    }
}

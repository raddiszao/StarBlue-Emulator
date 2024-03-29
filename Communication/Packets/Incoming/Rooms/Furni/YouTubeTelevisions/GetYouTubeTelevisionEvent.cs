﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;
using StarBlue.HabboHotel.Items.Televisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    internal class GetYouTubeTelevisionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int ItemId = Packet.PopInt();
            ICollection<TelevisionItem> Videos = StarBlueServer.GetGame().GetTelevisionManager().TelevisionList;
            if (Videos.Count == 0)
            {
                Session.SendNotification("Oh, Parece que el Administrador de Sistema del Hotel no ha añadido ningun video! :(");
                return;
            }

            Dictionary<int, TelevisionItem> dict = StarBlueServer.GetGame().GetTelevisionManager()._televisions;
            foreach (TelevisionItem value in RandomValues(dict).Take(1))
            {
                Session.SendMessage(new GetYouTubeVideoComposer(ItemId, value.YouTubeId));
            }

            Session.SendMessage(new GetYouTubePlaylistComposer(ItemId, Videos));
        }

        public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new Random();
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }
}
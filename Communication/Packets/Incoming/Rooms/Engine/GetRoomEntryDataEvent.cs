using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Poll;
using StarBlue.Communication.Packets.Outgoing.Rooms.Polls;
using StarBlue.Communication.Packets.Outgoing.SMS;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Polls;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Habbo Habbo = Session.GetHabbo();
            if (Session == null || Habbo == null)
            {
                return;
            }

            Room Room = Habbo.CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser ThisUser = Session.GetRoomUser();
            if (ThisUser == null)
            {
                if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
                {
                    Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                    return;
                }
            }
            else
            {
                return;
            }

            ICollection<RoomUser> RoomUsers = Room.GetRoomUserManager().GetUserList();
            Session.SendQueue(new HeightMapComposer(Room));
            Session.SendQueue(new FloorHeightMapComposer(Room.GetGameMap().Model.GetRelativeHeightmap(), Room.GetGameMap().StaticModel.WallHeight));
            Session.SendQueue(new UsersComposer(RoomUsers.Where(x => x != null).ToList()));

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null)
                {
                    continue;
                }

                if (User.IsBot && User.BotData.DanceId > 0)
                {
                    Session.SendQueue(new DanceComposer(User, User.BotData.DanceId));
                }
                else if (!User.IsBot && !User.IsPet && User.IsDancing)
                {
                    Session.SendQueue(new DanceComposer(User, User.DanceId));
                }

                if (User.IsAsleep)
                {
                    Session.SendQueue(new SleepComposer(User, true));
                }

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    Session.SendQueue(new CarryObjectComposer(User.VirtualId, User.CarryItemID));
                }

                if (!User.IsBot && !User.IsPet && User.CurrentEffect > 0 && User.GetClient().GetHabbo().Effects() != null)
                {
                    Session.SendQueue(new AvatarEffectComposer(User.VirtualId, User.CurrentEffect));
                }
            }

            Session.SendQueue(new ObjectsComposer(Room.RoomData.HideWired ? Room.GetRoomItemHandler().GetFloor.Where(Item => !Item.IsWired && Item.Data.Id != 716132).ToArray() : Room.GetRoomItemHandler().GetFloor.ToArray(), Room));
            Session.SendQueue(new ItemsComposer(Room.GetRoomItemHandler().GetWall.ToArray(), Room));
            Session.SendQueue(new UserUpdateComposer(RoomUsers.ToList()));
            Session.SendQueue(new RoomEntryInfoComposer(Room.Id, Session.GetHabbo().Username == Room.RoomData.OwnerName));
            Session.SendQueue(new RoomVisualizationSettingsComposer(Room.RoomData.WallThickness, Room.RoomData.FloorThickness, StarBlueServer.EnumToBool(Room.RoomData.Hidewall.ToString())));

            if (Habbo.GetStats().QuestID > 0)
            {
                StarBlueServer.GetGame().GetQuestManager().QuestReminder(Session, Habbo.GetStats().QuestID);
            }

            if (ThisUser != null && Habbo.PetId == 0)
            {
                Session.SendQueue(new UserChangeComposer(ThisUser, false));
            }

            Session.SendQueue(new RoomEventComposer(Room.RoomData, Room.RoomData.Promotion));

            if (Habbo.Rank > 3 && !Habbo.StaffOk)
            {
                if (!Session.GetHabbo().SendWebPacket(new PinVerifyComposer("open")))
                    Session.SendQueue(new SMSVerifyComposer(1, 1));
            }

            if (Room.poolQuestion != string.Empty)
            {
                Session.SendQueue(new QuickPollMessageComposer(Room.poolQuestion));

                if (Room.yesPoolAnswers.Contains(Habbo.Id) || Room.noPoolAnswers.Contains(Habbo.Id))
                {
                    Session.SendQueue(new QuickPollResultsMessageComposer(Room.yesPoolAnswers.Count, Room.noPoolAnswers.Count));
                }
            }

            if (Room.GetWired() != null)
            {
                Room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, Habbo);
            }

            if (Room.ForSale && Room.SalePrice > 0 && (Room.GetRoomUserManager().GetRoomUserByHabbo(Room.RoomData.OwnerName) != null))
            {
                Session.SendWhisper("Esta sala está a venda por " + Room.SalePrice + " Duckets. Escreva :buyroom se quiser comprá-la!");
            }
            else if (Room.ForSale && Room.GetRoomUserManager().GetRoomUserByHabbo(Room.RoomData.OwnerName) == null)
            {
                foreach (RoomUser _User in Room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Habbo.Id)
                    {
                        _User.GetClient().SendWhisper("Esta sala não está a venda.");
                    }
                }
                Room.ForSale = false;
                Room.SalePrice = 0;
            }

            if (StarBlueServer.GetGame().GetPollManager().TryGetPollForRoom(Room.Id, out RoomPoll poll) && poll.Type == RoomPollType.Poll)
            {
                if (!Habbo.GetPolls().CompletedPolls.Contains(poll.Id))
                {
                    Session.SendQueue(new PollOfferComposer(poll));
                }
            }

            if (StarBlueServer.GetUnixTimestamp() < Habbo.FloodTime && Habbo.FloodTime != 0)
            {
                Session.SendQueue(new FloodControlComposer((int)Habbo.FloodTime - (int)StarBlueServer.GetUnixTimestamp()));
            }

            ConcurrentDictionary<int, RoomUser> RoomBots = Room.GetRoomUserManager().GetBots();
            if (RoomBots.Count > 0)
            {
                foreach (RoomUser Bot in RoomBots.Values.ToList())
                {
                    if (Bot == null || Bot.BotAI == null)
                    {
                        continue;
                    }

                    Bot.BotAI.OnUserEnterRoom(ThisUser);
                }
            }

            if (Habbo.Rank >= 12 && !Habbo.DisableForcedEffects && ThisUser != null)
            {
                ThisUser.ApplyEffect(102);
            }

            if (Habbo.Rank == 2 && !Habbo.DisableForcedEffects && ThisUser != null)
            {
                ThisUser.ApplyEffect(593);
            }

            if (StarBlueServer.GetUnixTimestamp() < Habbo.FloodTime && Habbo.FloodTime != 0)
            {
                Session.SendQueue(new FloodControlComposer((int)Habbo.FloodTime - (int)StarBlueServer.GetUnixTimestamp()));
            }

            Session.Flush();

            if (Habbo.GetMessenger() != null)
            {
                Habbo.GetMessenger().OnStatusChanged(true);
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("INSERT INTO user_roomvisits (user_id,room_id,entry_timestamp,exit_timestamp,hour,minute) VALUES ('" + Habbo.Id + "','" + Habbo.CurrentRoomId + "','" + StarBlueServer.GetUnixTimestamp() + "','0','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "');");// +
            }

            if (Room.RoomData.OwnerId != Habbo.Id)
            {
                Habbo.GetStats().RoomVisits += 1;
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomEntry", 1);
            }
        }
    }
}
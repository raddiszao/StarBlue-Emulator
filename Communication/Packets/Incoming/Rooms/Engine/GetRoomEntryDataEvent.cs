using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Poll;
using StarBlue.Communication.Packets.Outgoing.Rooms.Polls;
using StarBlue.Communication.Packets.Outgoing.SMS;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Polls;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Session == null || Session.GetHabbo() == null || Room == null)
                return;

           // if (Session.GetHabbo().InRoom)
           // {
             //   if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room OldRoom))
             //       return;

   //             if (OldRoom.GetRoomUserManager() != null)
     //               OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
       //     }

            if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;
            }

            Room.SendObjects(Session);
            if (Room.RoomData.HideWired)
                Room.SendMessage(Room.HideWiredMessages(true));

            try
            {
                if (Session.GetHabbo().GetMessenger() != null)
                {
                    Session.GetHabbo().GetMessenger().OnStatusChanged(true);
                }
            }
            catch { }

            if (Session.GetHabbo().GetStats().QuestID > 0)
            {
                StarBlueServer.GetGame().GetQuestManager().QuestReminder(Session, Session.GetHabbo().GetStats().QuestID);
            }

            Session.SendMessage(new RoomEntryInfoComposer(Room.RoomId, Room.CheckRights(Session, true)));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, StarBlueServer.EnumToBool(Room.Hidewall.ToString())));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);

            if (ThisUser != null && Session.GetHabbo().PetId == 0)
            {
                Room.SendMessage(new UserChangeComposer(ThisUser, false));
            }

            if (!Session.GetHabbo().Effects().HasEffect(0) && Session.GetHabbo().Rank < 2)
            {
                Session.GetHabbo().Effects().ApplyEffect(0);
            }

            Session.SendMessage(new RoomEventComposer(Room.RoomData, Room.RoomData.Promotion));

            if (Session.GetHabbo().Rank > 6 && !Session.GetHabbo().StaffOk)
                Session.SendMessage(new SMSVerifyComposer(1, 1));

            if (Room.poolQuestion != string.Empty)
            {
                Session.SendMessage(new QuickPollMessageComposer(Room.poolQuestion));

                if (Room.yesPoolAnswers.Contains(Session.GetHabbo().Id) || Room.noPoolAnswers.Contains(Session.GetHabbo().Id))
                {
                    Session.SendMessage(new QuickPollResultsMessageComposer(Room.yesPoolAnswers.Count, Room.noPoolAnswers.Count));
                }
            }

            if (Room.GetWired() != null)
            {
                Room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, Session.GetHabbo());
            }

            if (Room.ForSale && Room.SalePrice > 0 && (Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) != null))
            {
                Session.SendWhisper("Esta sala está a venda por " + Room.SalePrice + " Duckets. Escreva :buyroom se quiser comprá-la!");
            }
            else if (Room.ForSale && Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) == null)
            {
                foreach (RoomUser _User in Room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        _User.GetClient().SendWhisper("Esta sala não está a venda.");
                    }
                }
                Room.ForSale = false;
                Room.SalePrice = 0;
            }


            if (StarBlueServer.GetGame().GetPollManager().TryGetPollForRoom(Room.Id, out RoomPoll poll) && poll.Type == RoomPollType.Poll)
            {
                if (!Session.GetHabbo().GetPolls().CompletedPolls.Contains(poll.Id))
                {
                    Session.SendMessage(new PollOfferComposer(poll));
                }
            }

            if (StarBlueServer.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
            {
                Session.SendMessage(new FloodControlComposer((int)Session.GetHabbo().FloodTime - (int)StarBlueServer.GetUnixTimestamp()));
            }

            // if (Room.OwnerId == Session.GetHabbo().Id)
            //    {

            //         if (Session.GetHabbo()._NUX)
            //           {

            //List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();
            //RoomUser BotUser = Room.GetRoomUserManager().DeployBot(new RoomBot(0, Session.GetHabbo().CurrentRoomId, "welcome", "freeroam", "Frank", "Manager del hotel", "hr-3194-38-36.hd-180-1.ch-220-1408.lg-285-73.sh-906-90.ha-3129-73.fa-1206-73.cc-3039-73", 0, 0, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, 0, false, 0, false, 33), null);


            //      }
            //          else
            //          {
            //User has already gotten today's prize :(
            //             }
            //          }

            foreach (RoomUser Bot in Room.GetRoomUserManager().GetBots().Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                {
                    continue;
                }

                Bot.BotAI.OnUserEnterRoom(Session.GetRoomUser());
                Bot.ApplyEffect(187);
            }

            if (Session.GetHabbo().Rank >= 12 && !Session.GetHabbo().DisableForcedEffects && Session.GetRoomUser() != null)
            {
                Session.GetRoomUser().ApplyEffect(102);
            }

            if (Session.GetHabbo().Rank == 2 && !Session.GetHabbo().DisableForcedEffects && Session.GetRoomUser() != null)
            {
                Session.GetRoomUser().ApplyEffect(593);
            }

            if (StarBlueServer.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
            {
                Session.SendMessage(new FloodControlComposer((int)Session.GetHabbo().FloodTime - (int)StarBlueServer.GetUnixTimestamp()));
            }

            if (Room.UsersNow < 2)
                Room.GetRoomItemHandler().SetSpeed(Room.RoomData.RollerSpeed);
        }
    }
}
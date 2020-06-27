using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            string Name = packet.PopString();
            Name = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word) ? "Spam" : Name;
            string Description = packet.PopString();
            Description = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Description, out word) ? "Spam" : Description;
            int RoomId = packet.PopInt();
            int Colour1 = packet.PopInt();
            int Colour2 = packet.PopInt();
            int Unknown = packet.PopInt();
            if (session.GetHabbo().Credits < Convert.ToInt32(StarBlueServer.GetConfig().data["group.purchase.amount"]))
            {
                session.SendMessage(new BroadcastMessageAlertComposer("A group costs " + Convert.ToInt32(StarBlueServer.GetConfig().data["group.purchase.amount"]) + " credits! You only have " + session.GetHabbo().Credits + "!"));
                return;
            }
            else
            {
                session.GetHabbo().Credits -= Convert.ToInt32(StarBlueServer.GetConfig().data["group.purchase.amount"]);
                session.SendMessage(new CreditBalanceComposer(session.GetHabbo().Credits));
            }
            RoomData Room = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Room == null || Room.OwnerId != session.GetHabbo().Id || Room.Group != null)
            {
                return;
            }

            string Badge = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }
            if (!StarBlueServer.GetGame().GetGroupManager().TryCreateGroup(session.GetHabbo(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group Group))
            {
                session.SendNotification("An error occured whilst trying to create this group.\n\nTry again. If you get this message more than once, report it at the link below.");
                return;
            }

            session.SendMessage(new PurchaseOKComposer());
            Room.Group = Group;
            if (session.GetHabbo().CurrentRoomId != Room.Id)
            {
                session.SendMessage(new RoomForwardComposer(Room.Id));
            }

            session.SendMessage(new NewGroupInfoComposer(RoomId, Group.Id));

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Room.Id, out Room Instance))
            {
                return;
            }

            foreach (int UserId in Instance.UsersWithRights)
            {
                RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }

                session.SendMessage(new FlatControllerRemovedComposer(Instance, UserId));
                session.SendMessage(new RoomRightsListComposer(Instance));
                session.SendMessage(new UserUpdateComposer(Instance.GetRoomUserManager().GetUserList().ToList()));
            }

            if (Instance.UsersWithRights.Count > 0)
            {
                Instance.UsersWithRights.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM room_rights WHERE room_id=@rid");
                dbClient.AddParameter("rid", Room.Id);
                dbClient.RunQuery();
            }

        }
    }
}
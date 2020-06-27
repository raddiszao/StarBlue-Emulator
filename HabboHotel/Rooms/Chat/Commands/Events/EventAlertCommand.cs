using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "";
        public string Description => "Envia alerta de evento ao Hotel!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            string Message = CommandManager.MergeParams(Params, 1);

            Session.GetHabbo()._eventsopened++;

            RoomAccess Access = RoomAccessUtility.ToRoomAccess(0);
            Room.Access = Access;
            Room.RoomData.Access = Access;

            Room.SendMessage(new RoomSettingsSavedComposer(Room.RoomId));
            Room.SendMessage(new RoomInfoUpdatedComposer(Room.RoomId));
            Room.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, StarBlueServer.EnumToBool(Room.Hidewall.ToString())));

            if (Room.UsersMax < 200)
            {
                Room.UsersMax = 200;
                Room.RoomData.UsersMax = 200;
            }

            //StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("eventb", "Está rolando um novo evento " + Room.Name + "! Clique aqui para ir ao evento.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients)
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                if (!Client.GetHabbo().DisabledNotificationEvents)
                {
                    Client.SendMessage(new RoomNotificationComposer("Está rolando um novo evento!",
                                     "<b><font color=\"#0489B1\">" + Session.GetHabbo().Username + "</font></b> está promovendo um novo evento!<br>" +
                                     "Participe do evento e ganhe prêmios!<br><br>O evento será no quarto   <b><font color=\"#0489B1\">" + Room.Name + "</font></b><br><br>" +
                                     "Se quiser ignorar este alerta, digite  <b><font color=\"#0489B1\">:disableevents</font></b>",
                                     "eventimage", "Participar do evento", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                }
            }

            LogEvent(Session.GetHabbo().Id, Room.Id, Message);
        }

        public void LogEvent(int MasterID, int RoomID, string Message)
        {
            DateTime Now = DateTime.Now;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO event_logs VALUES (NULL, " + MasterID + ", " + RoomID + ", @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }


    }
}


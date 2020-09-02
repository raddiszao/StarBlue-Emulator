using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "";
        public string Description => "Envia alerta de evento ao Hotel!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            string Message = CommandManager.MergeParams(Params, 1);

            Session.GetHabbo()._eventsopened++;

            Room.RoomData.Access = RoomAccess.OPEN;

            Room.SendMessage(new RoomSettingsSavedComposer(Room.Id));
            Room.SendMessage(new RoomInfoUpdatedComposer(Room.Id));
            Room.SendMessage(new RoomVisualizationSettingsComposer(Room.RoomData.WallThickness, Room.RoomData.FloorThickness, StarBlueServer.EnumToBool(Room.RoomData.Hidewall.ToString())));

            if (Room.RoomData.UsersMax < 50)
            {
                Room.RoomData.UsersMax = 50;
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
                                     "Participe do evento e ganhe prêmios!<br><br>O evento será no quarto   <b><font color=\"#0489B1\">" + Room.RoomData.Name + "</font></b>.<br><br>" +
                                     "<b>Clique no botão abaixo e participe!</b><br><br>" +
                                     "Se quiser ignorar este alerta, digite  <b><font color=\"#0489B1\">:disableevents</font></b><br><br><i>" + Session.GetHabbo().Username + "</i>",
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


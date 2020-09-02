using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.FloorPlan;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Rooms.FloorPlan
{
    internal class FloorPlanEditorRoomPropertiesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            DynamicRoomModel Model = Room.GetGameMap().Model;
            if (Model == null)
            {
                return;
            }

            ICollection<Item> FloorItems = Room.GetRoomItemHandler().GetFloor;

            Session.SendMessage(new FloorPlanFloorMapComposer(FloorItems));
            Session.SendMessage(new FloorPlanSendDoorComposer(Model.DoorX, Model.DoorY, Model.DoorOrientation));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.RoomData.WallThickness, Room.RoomData.FloorThickness, StarBlueServer.EnumToBool(Room.RoomData.Hidewall.ToString())));
        }
    }
}

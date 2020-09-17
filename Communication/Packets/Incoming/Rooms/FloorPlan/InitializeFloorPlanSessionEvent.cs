namespace StarBlue.Communication.Packets.Incoming.Rooms.FloorPlan
{
    internal class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            //Session.SendNotif("WARNING - THIS TOOL IS IN BETA, IT COULD CORRUPT YOUR ROOM IF YOU CONFIGURE THE MAP WRONG OR DISCONNECT YOU.");
        }
    }
}

using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    internal class ToggleMoodlightEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            Item Item = Room.GetRoomItemHandler().GetItem(Room.MoodlightData.ItemId);
            if (Item == null || Item.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
            {
                return;
            }

            if (Room.MoodlightData.Enabled)
            {
                Room.MoodlightData.Disable();
            }
            else
            {
                Room.MoodlightData.Enable();
            }

            Item.ExtraData = Room.MoodlightData.GenerateExtraData();
            Item.UpdateState();
        }
    }
}

using StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    internal class GetStickyNoteEvent : IPacketEvent
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

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null || Item.GetBaseItem().InteractionType != InteractionType.POSTIT)
            {
                return;
            }

            Session.SendMessage(new StickyNoteComposer(Item.Id.ToString(), Item.ExtraData));
        }
    }
}
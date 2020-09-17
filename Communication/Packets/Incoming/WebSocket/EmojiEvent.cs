using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.WebSocket;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.Packets.Incoming.WebSocket
{
    class EmojiEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, MessageWebEvent Packet)
        {
            if (Session == null)
                return;

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null)
                return;

            if (StarBlueServer.Now() - Client.GetHabbo().emojiTime < 8000)
            {
                Client.SendWhisper("Aguarde 8 segundos para enviar outro emoji.", 34);
                return;
            }

            int EmojiID = Packet.PopInt();
            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().CurrentRoom == null)
            {
                return;
            }

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room != null)
            {
                if (Room.RoomData.RoomMuted && !Client.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    return;

                string Username = "<img src='/swf/c_images/emoji/" + EmojiID + ".png' height='24' width='24'><br><br>      ";
                RoomUser TargetUser = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Username);
                if (Room != null && TargetUser != null)
                {
                    Room.SendMessage(new UserNameChangeComposer(Client.GetHabbo().CurrentRoomId, TargetUser.VirtualId, Username));
                    string Message = " ";
                    Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, TargetUser.LastBubble));
                    TargetUser.SendNamePacket();
                    Client.GetHabbo().emojiTime = StarBlueServer.Now();
                }
            }
        }
    }
}

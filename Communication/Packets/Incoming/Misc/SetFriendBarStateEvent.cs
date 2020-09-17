using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.Users.Messenger.FriendBar;

namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class SetFriendBarStateEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            Session.GetHabbo().FriendbarState = FriendBarStateUtility.GetEnum(Packet.PopInt());

            Session.GetHabbo().SendWebPacket(new ChangeEmojiStateComposer(Session.GetHabbo().FriendbarState == FriendBarState.CLOSED ? "close" : "open"));

            Session.SendMessage(new SoundSettingsComposer(Session.GetHabbo().ClientVolume, Session.GetHabbo().ChatPreference, Session.GetHabbo().AllowMessengerInvites, Session.GetHabbo().FocusPreference, FriendBarStateUtility.GetInt(Session.GetHabbo().FriendbarState)));
        }
    }
}

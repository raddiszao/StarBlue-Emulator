
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    public class UserPerksComposer : MessageComposer
    {
        private Habbo Habbo { get; }

        public UserPerksComposer(Habbo Habbo)
            : base(Composers.UserPerksMessageComposer)
        {
            this.Habbo = Habbo;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(14); // Count
            packet.WriteString("USE_GUIDE_TOOL");
            packet.WriteString("");
            packet.WriteBoolean(Habbo.Rank > 3 || Habbo._guidelevel > 0);
            packet.WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("GIVE_GUIDE_TOURS");
            packet.WriteString("requirement.unfulfilled.helper_le");
            packet.WriteBoolean(true);
            packet.WriteString("JUDGE_CHAT_REVIEWS");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("VOTE_IN_COMPETITIONS");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("CALL_ON_HELPERS");
            packet.WriteString("true"); // ??
            packet.WriteBoolean(true);
            packet.WriteString("CITIZEN");
            packet.WriteString("");
            packet.WriteBoolean(true);
            packet.WriteString("TRADE");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("HEIGHTMAP_EDITOR_BETA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("BUILDER_AT_WORK");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("NAVIGATOR_PHASE_ONE_2014");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("CAMERA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("NAVIGATOR_PHASE_TWO_2014");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("MOUSE_ZOOM");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("HABBO_CLUB_OFFER_BETA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
        }
    }
}
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class GetGuestRoomResultComposer : MessageComposer
    {
        private Habbo Habbo { get; }
        private RoomData Data { get; }
        private bool isLoading { get; }
        private bool checkEntry { get; }

        public GetGuestRoomResultComposer(GameClient Session, RoomData Data, bool isLoading, bool checkEntry)
            : base(Composers.GetGuestRoomResultMessageComposer)
        {
            this.Habbo = Session.GetHabbo();
            this.Data = Data;
            this.isLoading = isLoading;
            this.checkEntry = checkEntry;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(isLoading);
            packet.WriteInteger(Data.Id);
            packet.WriteString(Data.Name);
            if (Data.Type == "public")
            {
                packet.WriteInteger(0);
                packet.WriteString("Quarto Público");
            }
            else
            {
                packet.WriteInteger(Data.OwnerId);
                packet.WriteString(Data.OwnerName);
            }

            packet.WriteInteger(Habbo.GetPermissions().HasRight("room_any_rights") ? 0 : RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            packet.WriteInteger(Data.UsersNow);
            packet.WriteInteger(Data.UsersMax);
            packet.WriteString(Data.Description);
            packet.WriteInteger(Data.TradeSettings);
            packet.WriteInteger(2);
            packet.WriteInteger(Data.Score);
            packet.WriteInteger(Data.Category);

            packet.WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
                packet.WriteString(Tag);
            }

            if (Data.Group != null && Data.Promotion != null)
            {
                packet.WriteInteger(62);//What?

                packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);

                packet.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                packet.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else if (Data.Group != null && Data.Promotion == null)
            {
                packet.WriteInteger(58);//What?
                packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
            else if (Data.Group == null && Data.Promotion != null)
            {
                packet.WriteInteger(60);//What?
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                packet.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else
            {
                packet.WriteInteger(56);//What?
            }


            packet.WriteBoolean(checkEntry);
            packet.WriteBoolean(StarBlueServer.GetGame().GetNavigator().TryGetStaffPickedRoom(Data.Id, out StaffPick staffPick));
            packet.WriteBoolean(Data.Group != null && Data.Group.IsMember(Habbo.Id)); //Unknown
            packet.WriteBoolean(Data.RoomMuted); //Unknown

            packet.WriteInteger(Data.WhoCanMute);
            packet.WriteInteger(Data.WhoCanKick);
            packet.WriteInteger(Data.WhoCanBan);

            packet.WriteBoolean(Habbo.GetPermissions().HasRight("mod_tool") || Data.OwnerName == Habbo.Username);//Room muting.
            packet.WriteInteger(Data.chatMode);
            packet.WriteInteger(Data.chatSize);
            packet.WriteInteger(Data.chatSpeed);
            packet.WriteInteger(Data.chatDistance);//Hearing distance
            packet.WriteInteger(Data.extraFlood);//Flood!!
        }
    }
}
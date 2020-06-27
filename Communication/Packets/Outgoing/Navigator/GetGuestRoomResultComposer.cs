﻿using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using StarBlue.HabboHotel.Rooms;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    class GetGuestRoomResultComposer : ServerPacket
    {
        public GetGuestRoomResultComposer(GameClient Session, RoomData Data, Boolean isLoading, Boolean checkEntry)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            base.WriteBoolean(isLoading);
            base.WriteInteger(Data.Id);
            base.WriteString(Data.Name);
            base.WriteInteger(Data.OwnerId);
            base.WriteString(Data.OwnerName);
            base.WriteInteger(Session.GetHabbo().GetPermissions().HasRight("room_any_rights") ? 0 : RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            base.WriteInteger(Data.UsersNow);
            base.WriteInteger(Data.UsersMax);
            base.WriteString(Data.Description);
            base.WriteInteger(Data.TradeSettings);
            base.WriteInteger(Data.Score);
            base.WriteInteger(0);//Top rated room rank.
            base.WriteInteger(Data.Category);

            base.WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
                base.WriteString(Tag);
            }

            if (Data.Group != null && Data.Promotion != null)
            {
                base.WriteInteger(62);//What?

                base.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                base.WriteString(Data.Group == null ? "" : Data.Group.Name);
                base.WriteString(Data.Group == null ? "" : Data.Group.Badge);

                base.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                base.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                base.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else if (Data.Group != null && Data.Promotion == null)
            {
                base.WriteInteger(58);//What?
                base.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                base.WriteString(Data.Group == null ? "" : Data.Group.Name);
                base.WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
            else if (Data.Group == null && Data.Promotion != null)
            {
                base.WriteInteger(60);//What?
                base.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                base.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                base.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else
            {
                base.WriteInteger(56);//What?
            }


            base.WriteBoolean(checkEntry);

            if (!StarBlueServer.GetGame().GetNavigator().TryGetStaffPickedRoom(Data.Id, out StaffPick staffPick))
            {
                WriteBoolean(false);
            }
            else
            {
                WriteBoolean(true);
            }

            base.WriteBoolean(false); //Unknown
            base.WriteBoolean(false); //Unknown

            base.WriteInteger(Data.WhoCanMute);
            base.WriteInteger(Data.WhoCanKick);
            base.WriteInteger(Data.WhoCanBan);

            base.WriteBoolean(Session.GetHabbo().GetPermissions().HasRight("mod_tool") || Data.OwnerName == Session.GetHabbo().Username);//Room muting.
            base.WriteInteger(Data.chatMode);
            base.WriteInteger(Data.chatSize);
            base.WriteInteger(Data.chatSpeed);
            base.WriteInteger(Data.extraFlood);//Hearing distance
            base.WriteInteger(Data.chatDistance);//Flood!!
        }
    }
}
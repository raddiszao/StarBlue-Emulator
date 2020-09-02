
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(Room Room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            base.WriteInteger(Room.Id);
            base.WriteString(Room.RoomData.Name);
            base.WriteString(Room.RoomData.Description);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Room.RoomData.Access));
            base.WriteInteger(Room.RoomData.Category);
            base.WriteInteger(Room.RoomData.UsersMax);
            base.WriteInteger(((Room.RoomData.Model.MapSizeX * Room.RoomData.Model.MapSizeY) > 100) ? 50 : 25);

            base.WriteInteger(Room.RoomData.Tags.Count);
            foreach (string Tag in Room.RoomData.Tags.ToArray())
            {
                base.WriteString(Tag);
            }

            base.WriteInteger(Room.RoomData.TradeSettings); //Trade
            base.WriteInteger(Room.RoomData.AllowPets); // allows pets in room - pet system lacking, so always off
            base.WriteInteger(Room.RoomData.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            base.WriteInteger(Room.RoomData.RoomBlockingEnabled);
            base.WriteInteger(Room.RoomData.Hidewall);
            base.WriteInteger(Room.RoomData.WallThickness);
            base.WriteInteger(Room.RoomData.FloorThickness);

            base.WriteInteger(Room.RoomData.chatMode);//Chat mode
            base.WriteInteger(Room.RoomData.chatSize);//Chat size
            base.WriteInteger(Room.RoomData.chatSpeed);//Chat speed
            base.WriteInteger(Room.RoomData.chatDistance);//Hearing Distance
            base.WriteInteger(Room.RoomData.extraFlood);//Additional Flood
            base.WriteBoolean(true);
            base.WriteInteger(Room.RoomData.WhoCanMute); // who can mute
            base.WriteInteger(Room.RoomData.WhoCanKick); // who can kick
            base.WriteInteger(Room.RoomData.WhoCanBan); // who can ban

        }
    }
}

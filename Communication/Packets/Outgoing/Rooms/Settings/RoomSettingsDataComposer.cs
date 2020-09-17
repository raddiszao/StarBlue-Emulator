
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsDataComposer : MessageComposer
    {
        public Room Room { get; }
        public RoomSettingsDataComposer(Room room)
            : base(Composers.RoomSettingsDataMessageComposer)
        {
            this.Room = room;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Room.Id);
            packet.WriteString(Room.RoomData.Name);
            packet.WriteString(Room.RoomData.Description);
            packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Room.RoomData.Access));
            packet.WriteInteger(Room.RoomData.Category);
            packet.WriteInteger(Room.RoomData.UsersMax);
            packet.WriteInteger(((Room.RoomData.Model.MapSizeX * Room.RoomData.Model.MapSizeY) > 100) ? 50 : 25);

            packet.WriteInteger(Room.RoomData.Tags.Count);
            foreach (string Tag in Room.RoomData.Tags.ToArray())
            {
                packet.WriteString(Tag);
            }

            packet.WriteInteger(Room.RoomData.TradeSettings); //Trade
            packet.WriteInteger(Room.RoomData.AllowPets); // allows pets in room - pet system lacking, so always off
            packet.WriteInteger(Room.RoomData.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            packet.WriteInteger(Room.RoomData.RoomBlockingEnabled);
            packet.WriteInteger(Room.RoomData.Hidewall);
            packet.WriteInteger(Room.RoomData.WallThickness);
            packet.WriteInteger(Room.RoomData.FloorThickness);

            packet.WriteInteger(Room.RoomData.chatMode);//Chat mode
            packet.WriteInteger(Room.RoomData.chatSize);//Chat size
            packet.WriteInteger(Room.RoomData.chatSpeed);//Chat speed
            packet.WriteInteger(Room.RoomData.chatDistance);//Hearing Distance
            packet.WriteInteger(Room.RoomData.extraFlood);//Additional Flood
            packet.WriteBoolean(true);
            packet.WriteInteger(Room.RoomData.WhoCanMute); // who can mute
            packet.WriteInteger(Room.RoomData.WhoCanKick); // who can kick
            packet.WriteInteger(Room.RoomData.WhoCanBan); // who can ban

        }
    }
}

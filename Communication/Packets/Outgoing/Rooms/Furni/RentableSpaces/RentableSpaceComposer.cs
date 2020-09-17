namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces
{
    public class RentableSpaceComposer : MessageComposer
    {
        private bool Rented { get; }
        private int ErrorId { get; }
        private int ExpireSecs { get; }
        private int OwnerId { get; }
        private int Price { get; }
        private bool Val { get; }

        public RentableSpaceComposer(bool Rented, int ErrorId, int OwnerId, int ExpireSecs, int Price)
            : base(Composers.RentableSpaceMessageComposer)
        {
            this.Rented = Rented;
            this.ErrorId = ErrorId;
            this.OwnerId = OwnerId;
            this.ExpireSecs = ExpireSecs;
            this.Price = Price;
            this.Val = false;
        }

        public RentableSpaceComposer(int OwnerId, int ExpireSecs)
            : base(Composers.RentableSpaceMessageComposer)
        {
            this.OwnerId = OwnerId;
            this.ExpireSecs = ExpireSecs;
            this.Val = true;
        }

        public override void Compose(Composer packet)
        {
            if (!Val)
            {
                packet.WriteBoolean(Rented); //Is rented??
                packet.WriteInteger(ErrorId);
                packet.WriteInteger(OwnerId);
                packet.WriteString(StarBlueServer.GetUsernameById(OwnerId));
                packet.WriteInteger(ExpireSecs);
                packet.WriteInteger(Price);
            }
            else
            {
                packet.WriteBoolean(true); //Is rented??
                packet.WriteInteger(0);
                packet.WriteInteger(OwnerId);
                packet.WriteString(StarBlueServer.GetUsernameById(OwnerId));
                packet.WriteInteger(ExpireSecs);
                packet.WriteInteger(0);
            }
        }
    }
}
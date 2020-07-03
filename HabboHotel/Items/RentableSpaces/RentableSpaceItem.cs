namespace StarBlue.HabboHotel.Items.RentableSpaces
{
    public class RentableSpaceItem
    {
        private int _itemId;
        private int _ownerId;
        private int _expireStamp;
        private int _price;

        public RentableSpaceItem(int ItemId, int OwnerId, int ExpireStamp, int Price)
        {
            _itemId = ItemId;
            _ownerId = OwnerId;
            _expireStamp = ExpireStamp;
            _price = Price;
        }

        public bool IsRented()
        {
            return _expireStamp > StarBlueServer.GetUnixTimestamp();
        }

        public bool Rented => IsRented();

        public int ItemId
        {
            get => _itemId;
            set => _itemId = value;
        }


        public int OwnerId
        {
            get => _ownerId;
            set => _ownerId = value;
        }

        public int ExpireStamp
        {
            get => _expireStamp;
            set => _expireStamp = value;
        }

        public int Price
        {
            get => _price;
            set => _price = value;
        }

        public int GetExpireSeconds()
        {
            int i = _expireStamp - (int)StarBlueServer.GetUnixTimestamp();
            return i > 0 ? i : 0;
        }

    }
}
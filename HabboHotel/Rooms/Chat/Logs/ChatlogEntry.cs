using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Logs
{
    public sealed class ChatlogEntry
    {
        private int _playerId;
        private int _roomId;
        private string _message;
        private double _timestamp;

        private WeakReference _playerReference;
        private WeakReference _roomReference;

        public ChatlogEntry(int PlayerId, int RoomId, string Message, double Timestamp, Habbo Player = null, RoomData Instance = null)
        {
            _playerId = PlayerId;
            _roomId = RoomId;
            _message = Message;
            _timestamp = Timestamp;

            if (Player != null)
            {
                _playerReference = new WeakReference(Player);
            }

            if (Instance != null)
            {
                _roomReference = new WeakReference(Instance);
            }
        }

        public int PlayerId => _playerId;

        public int RoomId => _roomId;

        public string Message => _message;

        public double Timestamp => _timestamp;

        public Habbo PlayerNullable()
        {
            if (_playerReference.IsAlive)
            {
                Habbo PlayerObj = (Habbo)_playerReference.Target;

                return PlayerObj;
            }

            return null;
        }

        public Room RoomNullable()
        {
            if (_roomReference.IsAlive)
            {
                Room RoomObj = (Room)_roomReference.Target;
                if (RoomObj.mDisposed)
                {
                    return null;
                }

                return RoomObj;
            }
            return null;
        }
    }
}

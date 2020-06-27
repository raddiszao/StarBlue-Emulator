namespace StarBlue.HabboHotel.Camera
{
    public class CameraPhotoPreview
    {
        private int _photoId;
        private int _creatorId;
        private long _createdAt;

        public int Id
        {
            get
            {
                return _photoId;
            }
        }

        public int CreatorId
        {
            get
            {
                return _creatorId;
            }
        }

        public long CreatedAt
        {
            get
            {
                return _createdAt;
            }
        }

        public CameraPhotoPreview(int photoId, int creatorId, long createdAt)
        {
            _photoId = photoId;
            _creatorId = creatorId;
            _createdAt = createdAt;
        }

        public CameraPhotoPreview(int photoId)
        {
            _photoId = photoId;
        }

    }
}
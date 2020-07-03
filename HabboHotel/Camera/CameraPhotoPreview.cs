namespace StarBlue.HabboHotel.Camera
{
    public class CameraPhotoPreview
    {
        private int _photoId;
        private int _creatorId;
        private long _createdAt;

        public int Id => _photoId;

        public int CreatorId => _creatorId;

        public long CreatedAt => _createdAt;

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
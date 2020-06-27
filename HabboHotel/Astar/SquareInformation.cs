using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Runtime.InteropServices;

namespace StarBlue.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SquareInformation
    {
        private int mX;
        private int mY;
        private SquarePoint[] mPos;
        private SquarePoint mTarget;
        private SquarePoint mPoint;

        public SquareInformation(RoomUser User, Vector2D From, SquarePoint pTarget, ModelInfo pMap, bool pUserOverride, bool CalculateDiagonal, Gamemap mMap)
        {
            mX = From.X;
            mY = From.Y;
            mTarget = pTarget;
            mPoint = new SquarePoint(User, From, pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y), pUserOverride, mMap);
            mPos = new SquarePoint[8];
            if (CalculateDiagonal)
            {
                mPos[1] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y - 1), pUserOverride, mMap);
                mPos[3] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y + 1), pUserOverride, mMap);
                mPos[5] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y + 1), pUserOverride, mMap);
                mPos[7] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y - 1), pUserOverride, mMap);
            }
            mPos[0] = new SquarePoint(User, new Vector2D(From.X, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y - 1), pUserOverride, mMap);
            mPos[2] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y), pUserOverride, mMap);
            mPos[4] = new SquarePoint(User, new Vector2D(From.X, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y + 1), pUserOverride, mMap);
            mPos[6] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y), pUserOverride, mMap);
        }

        public SquarePoint Pos(int val)
        {
            return mPos[val];
        }

        public SquarePoint Point
        {
            get
            {
                return mPoint;
            }
        }
    }
}
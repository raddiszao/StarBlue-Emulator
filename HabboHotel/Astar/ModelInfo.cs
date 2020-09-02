using System.Runtime.InteropServices;

namespace StarBlue.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ModelInfo
    {
        private readonly byte[,] mMap;
        private readonly int mMaxX;
        private readonly int mMaxY;
        internal ModelInfo(int MaxX, int MaxY, byte[,] Map)
        {
            mMap = Map;
            mMaxX = MaxX;
            mMaxY = MaxY;
        }

        public byte GetState(int x, int y)
        {
            if ((x >= this.mMaxX) || (x < 0))
            {
                return 0;
            }
            if ((y >= this.mMaxY) || (y < 0))
            {
                return 0;
            }
            return this.mMap[x, y];
        }
    }
}
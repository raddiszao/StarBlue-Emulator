using System.Runtime.InteropServices;

namespace StarBlue.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HeightInfo
    {
        private readonly double[,] mMap;
        private readonly int mMaxX;
        private readonly int mMaxY;

        public HeightInfo(int MaxX, int MaxY, double[,] Map)
        {
            mMap = Map;
            mMaxX = MaxX;
            mMaxY = MaxY;
        }

        public double GetState(int x, int y)
        {
            if ((x >= this.mMaxX) || (x < 0))
            {
                return 0.0;
            }
            if ((y >= this.mMaxY) || (y < 0))
            {
                return 0.0;
            }
            return this.mMap[x, y];
        }
    }
}
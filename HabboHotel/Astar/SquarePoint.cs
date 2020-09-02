using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace StarBlue.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SquarePoint
    {
        private RoomUser mUser;
        private readonly int mX;
        private readonly int mY;
        private readonly double mDistance;
        private readonly byte mSquareData;
        private readonly bool mInUse;
        private readonly bool mOverride;
        private readonly bool mLastStep;
        private Gamemap mMap;

        public SquarePoint(RoomUser User, Vector2D From, int pTargetX, int pTargetY, byte SquareData, bool pOverride, Gamemap pGameMap)
        {
            this.mUser = User;
            this.mX = From.X;
            this.mY = From.Y;
            this.mSquareData = SquareData;
            this.mInUse = true;
            this.mOverride = pOverride;
            this.mDistance = 0.0;
            this.mLastStep = (From.X == pTargetX) && (From.Y == pTargetY);
            this.mDistance = DreamPathfinder.GetDistance(From.X, From.Y, pTargetX, pTargetY);
            this.mMap = pGameMap;
        }

        public int X
        {
            get
            {
                return this.mX;
            }
        }

        public int Y
        {
            get
            {
                return this.mY;
            }
        }

        public double GetDistance
        {
            get
            {
                return this.mDistance;
            }
        }

        public bool CanWalk
        {
            get
            {
                if (!this.mLastStep)
                {
                    if (!this.mOverride)
                    {
                        return ((this.mSquareData == 1) || (this.mSquareData == 4));
                    }

                    return true;
                }

                if (mLastStep)
                {
                    if (mMap != null)
                    {
                        List<Item> Items = mMap.GetAllRoomItemForSquare(X, Y);
                        if (Items.Count > 0)
                        {
                            bool HasGroupGate = Items.ToList().Where(x => x != null && x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE).Count() > 0;
                            if (HasGroupGate)
                            {
                                Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                                if (I != null)
                                {
                                    if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(I.GroupId, out Group Group))
                                        return false;

                                    if (mUser.GetClient() == null || mUser.GetClient().GetHabbo() == null)
                                        return false;

                                    if (Group.IsMember(mUser.GetClient().GetHabbo().Id))
                                        return true;

                                    else
                                        return false;
                                }
                            }

                        }
                    }
                }

                if (!this.mOverride)
                {
                    if (this.mSquareData == 3)
                    {
                        return true;
                    }
                    if (this.mSquareData == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                return false;
            }
        }
        public bool InUse
        {
            get
            {
                return this.mInUse;
            }
        }
    }
}
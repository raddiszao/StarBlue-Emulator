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
        private int mX;
        private int mY;
        private double mDistance;
        private byte mSquareData;
        private bool mInUse;
        private bool mOverride;
        private bool mLastStep;
        private Gamemap mMap;

        public SquarePoint(RoomUser User, Vector2D From, int pTargetX, int pTargetY, byte SquareData, bool pOverride, Gamemap pGameMap)
        {
            mUser = User;
            mX = From.X;
            mY = From.Y;
            mSquareData = SquareData;
            mInUse = true;
            mOverride = pOverride;
            mDistance = 0.0;
            mLastStep = (From.X == pTargetX) && (From.Y == pTargetY);
            mDistance = DreamPathfinder.GetDistance(From.X, From.Y, pTargetX, pTargetY);
            mMap = pGameMap;
        }

        public int X
        {
            get
            {
                return mX;
            }
        }

        public int Y
        {
            get
            {
                return mY;
            }
        }

        public double GetDistance
        {
            get
            {
                return mDistance;
            }
        }

        public bool CanWalk
        {
            get
            {
                if (!mLastStep)
                {
                    if (!mOverride)
                    {
                        return ((mSquareData == 1) || (mSquareData == 4));
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
                                    {
                                        return false;
                                    }

                                    if (mUser.GetClient() == null || mUser.GetClient().GetHabbo() == null)
                                    {
                                        return false;
                                    }

                                    if (Group.IsMember(mUser.GetClient().GetHabbo().Id))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }

                            bool HasHcGate = Items.ToList().Where(x => x.GetBaseItem().InteractionType == InteractionType.HCGATE).ToList().Count() > 0;
                            if (HasHcGate)
                            {
                                Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.HCGATE);
                                if (I != null)
                                {
                                    if (mUser.GetClient() == null || mUser.GetClient().GetHabbo() == null)
                                    {
                                        return false;
                                    }

                                    bool IsHc = mUser.GetClient().GetHabbo().GetClubManager().HasSubscription("habbo_vip");
                                    if (!IsHc)
                                    {
                                        return false;
                                    }

                                    if (mUser.GetClient().GetHabbo().GetClubManager().HasSubscription("habbo_vip"))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }

                            bool HasVIPGate = Items.ToList().Where(x => x.GetBaseItem().InteractionType == InteractionType.VIPGATE).ToList().Count() > 0;
                            if (HasVIPGate)
                            {
                                Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.VIPGATE);
                                if (I != null)
                                {
                                    var IsVIP = mUser.GetClient().GetHabbo().GetClubManager().HasSubscription("club_vip");
                                    if (!IsVIP)
                                    {
                                        return false;
                                    }

                                    if (mUser.GetClient() == null || mUser.GetClient().GetHabbo() == null)
                                    {
                                        return false;
                                    }

                                    if (mUser.GetClient().GetHabbo().GetClubManager().HasSubscription("club_vip"))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!mOverride)
                {
                    if (mSquareData == 3)
                    {
                        return true;
                    }
                    if (mSquareData == 1)
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
                return mInUse;
            }
        }
    }
}
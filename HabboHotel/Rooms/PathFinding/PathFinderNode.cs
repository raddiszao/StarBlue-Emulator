﻿using System;

namespace StarBlue.HabboHotel.Rooms.PathFinding
{
    public sealed class PathFinderNode : IComparable<PathFinderNode>
    {
        public Vector2D Position;
        public PathFinderNode Next;
        public int Cost = int.MaxValue;
        public bool InOpen = false;
        public bool InClosed = false;

        public PathFinderNode(Vector2D Position)
        {
            this.Position = Position;
        }

        public override bool Equals(object obj)
        {
            return (obj is PathFinderNode) && ((PathFinderNode)obj).Position.Equals(Position);
        }

        public bool Equals(PathFinderNode Breadcrumb)
        {
            return Breadcrumb.Position.Equals(Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public int CompareTo(PathFinderNode Other)
        {
            return Cost.CompareTo(Other.Cost);
        }
    }
}

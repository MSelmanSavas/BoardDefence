using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulDataTypes.Utils
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class DirectionUtils
    {
        public static Vector2Int None => Vector2Int.zero;
        public static Vector2Int Up => Vector2Int.up;
        public static Vector2Int Down => Vector2Int.down;
        public static Vector2Int Right => Vector2Int.right;
        public static Vector2Int Left => Vector2Int.left;
        public static Vector2Int UpLeft => Vector2Int.up + Vector2Int.left;
        public static Vector2Int UpRight => Vector2Int.up + Vector2Int.right;
        public static Vector2Int DownLeft => Vector2Int.down + Vector2Int.left;
        public static Vector2Int DownRight => Vector2Int.down + Vector2Int.right;

        public static readonly List<Direction> AllDirections = new List<Direction>
    {
        Direction.Up,
        Direction.Down,
        Direction.Right,
        Direction.Left,
        Direction.UpLeft,
        Direction.UpRight,
        Direction.DownLeft,
        Direction.DownRight
    };

        public static readonly List<Vector2Int> AllDirectionVector2Ints = new List<Vector2Int>
    {
        Up,
        Down,
        Right,
        Left,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    };

        public static Vector2 GetVector2FromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Vector2.left;
                case Direction.Right:
                    return Vector2.right;
                case Direction.Up:
                    return Vector2.up;
                case Direction.Down:
                    return Vector2.down;
                case Direction.UpLeft:
                    return (Vector2.up + Vector2.left).normalized;
                case Direction.UpRight:
                    return (Vector2.up + Vector2.right).normalized;
                case Direction.DownLeft:
                    return (Vector2.down + Vector2.left).normalized;
                case Direction.DownRight:
                    return (Vector2.down + Vector2.right).normalized;
                case Direction.None:
                    return Vector2.zero;
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2Int GetVector2IntFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.UpLeft:
                    return Vector2Int.up + Vector2Int.left;
                case Direction.UpRight:
                    return Vector2Int.up + Vector2Int.right;
                case Direction.DownLeft:
                    return Vector2Int.down + Vector2Int.left;
                case Direction.DownRight:
                    return Vector2Int.down + Vector2Int.right;
                case Direction.None:
                    return Vector2Int.zero;
                default:
                    return Vector2Int.zero;
            }
        }

        public static bool IsVertical(Direction direction)
        {
            return direction == Direction.Up || direction == Direction.Down;
        }

        public static bool IsHorizontal(Direction direction)
        {
            return direction == Direction.Right || direction == Direction.Left;
        }

        public static Direction Vector2IntToDirection(Vector2Int from, Vector2Int to)
        {
            Vector2 directionVector = to - from;

            directionVector.Normalize();

            float dot;

            float MaxInputSimilarity = 0;
            int MaxSimilarInputIndex = 0;

            for (int i = 0; i < AllDirections.Count; i++)
            {
                dot = Vector2.Dot(directionVector, DirectionUtils.GetVector2FromDirection(AllDirections[i]));

                if (dot > MaxInputSimilarity)
                {
                    MaxInputSimilarity = dot;
                    MaxSimilarInputIndex = i;
                }
            }

            return AllDirections[MaxSimilarInputIndex];
        }

        public static Direction Vector2IntToDirectionFast(Vector2Int from, Vector2Int to)
        {
            Vector2Int directionVector = to - from;

            return directionVector switch
            {
                { x: 0, y: > 0 } => Direction.Up,
                { x: 0, y: < 0 } => Direction.Down,
                { x: > 0, y: 0 } => Direction.Right,
                { x: < 0, y: 0 } => Direction.Left,
                { x: > 0, y: > 0 } => Direction.UpRight,
                { x: < 0, y: < 0 } => Direction.DownLeft,
                { x: < 0, y: > 0 } => Direction.UpLeft,
                { x: > 0, y: < 0 } => Direction.DownRight,
                _ => Direction.None,
            };
        }

        public static Direction Vector2ToDirection(Vector2 from, Vector2 to)
        {
            Vector2 directionVector = to - from;

            directionVector.Normalize();

            float dot;

            float MaxInputSimilarity = 0;
            int MaxSimilarInputIndex = 0;

            for (int i = 0; i < AllDirections.Count; i++)
            {
                dot = Vector2.Dot(directionVector, DirectionUtils.GetVector2FromDirection(AllDirections[i]));

                if (dot > MaxInputSimilarity)
                {
                    MaxInputSimilarity = dot;
                    MaxSimilarInputIndex = i;
                }
            }

            return AllDirections[MaxSimilarInputIndex];
        }

        public static Direction GetReverseDirection(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.UpLeft:
                    return Direction.DownRight;
                case Direction.UpRight:
                    return Direction.DownLeft;
                case Direction.DownLeft:
                    return Direction.UpRight;
                case Direction.DownRight:
                    return Direction.UpLeft;
                default:
                    return Direction.None;
            }
        }

        public static Direction GetClockwiseDirection(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Up;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Up:
                    return Direction.Right;
                case Direction.Down:
                    return Direction.Left;
                case Direction.UpLeft:
                    return Direction.UpRight;
                case Direction.UpRight:
                    return Direction.DownRight;
                case Direction.DownLeft:
                    return Direction.UpLeft;
                case Direction.DownRight:
                    return Direction.DownLeft;
                default:
                    return Direction.None;
            }
        }

        public static Direction GetAntiClockwiseDirection(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Right;
                case Direction.UpLeft:
                    return Direction.DownLeft;
                case Direction.UpRight:
                    return Direction.UpLeft;
                case Direction.DownLeft:
                    return Direction.DownRight;
                case Direction.DownRight:
                    return Direction.UpRight;
                default:
                    return Direction.None;
            }
        }

        public static bool CheckDirectionSimilarities(Direction firstDirection, List<Direction> checkDirections)
        {
            switch (firstDirection)
            {
                case Direction.Left:
                    if (checkDirections.Contains(Direction.Left) || checkDirections.Contains(Direction.Right))
                        return true;
                    break;
                case Direction.Right:
                    if (checkDirections.Contains(Direction.Left) || checkDirections.Contains(Direction.Right))
                        return true;
                    break;
                case Direction.Up:
                    if (checkDirections.Contains(Direction.Up) || checkDirections.Contains(Direction.Down))
                        return true;
                    break;
                case Direction.Down:
                    if (checkDirections.Contains(Direction.Up) || checkDirections.Contains(Direction.Down))
                        return true;
                    break;
                case Direction.UpLeft:
                    if (checkDirections.Contains(Direction.UpLeft) || checkDirections.Contains(Direction.DownRight))
                        return true;
                    break;
                case Direction.UpRight:
                    if (checkDirections.Contains(Direction.UpRight) || checkDirections.Contains(Direction.DownLeft))
                        return true;
                    break;
                case Direction.DownLeft:
                    if (checkDirections.Contains(Direction.UpRight) || checkDirections.Contains(Direction.DownLeft))
                        return true;
                    break;
                case Direction.DownRight:
                    if (checkDirections.Contains(Direction.UpLeft) || checkDirections.Contains(Direction.DownRight))
                        return true;
                    break;
            }

            return false;
        }
    }
}

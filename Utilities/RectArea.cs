using System;

namespace BongoCat_Like.Utilities
{
    public class RectArea
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public RectArea() : this(0, 0, 0, 0) { }

        public RectArea(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectArea(int left, int top, int right, int bottom, bool fromEdges)
        {
            if (fromEdges)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            else
            {
                X = left;
                Y = top;
                Width = right;
                Height = bottom;
            }
        }

        public int X
        {
            get => _x;
            set
            {
                _x = value;
                UpdateDerivedProperties();
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                UpdateDerivedProperties();
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateDerivedProperties();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                UpdateDerivedProperties();
            }
        }

        public int Left
        {
            get => _x;
            set
            {
                int right = Right;
                _x = value;
                _width = right - value;
                UpdateDerivedProperties();
            }
        }

        public int Top
        {
            get => _y;
            set
            {
                int bottom = Bottom;
                _y = value;
                _height = bottom - value;
                UpdateDerivedProperties();
            }
        }

        public int Right
        {
            get => _x + _width;
            set
            {
                _width = value - _x;
                UpdateDerivedProperties();
            }
        }

        public int Bottom
        {
            get => _y + _height;
            set
            {
                _height = value - _y;
                UpdateDerivedProperties();
            }
        }

        private void UpdateDerivedProperties()
        {
            if (_width < 0)
            {
                _x += _width;
                _width = -_width;
            }

            if (_height < 0)
            {
                _y += _height;
                _height = -_height;
            }
        }

        public override string ToString()
        {
            return $"RectArea(X={X}, Y={Y}, Width={Width}, Height={Height}, Left={Left}, Top={Top}, Right={Right}, Bottom={Bottom})";
        }

        public bool Contains(int x, int y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        public bool Contains(RectArea other)
        {
            return other.Left >= Left && other.Right <= Right &&
                   other.Top >= Top && other.Bottom <= Bottom;
        }

        public bool IntersectsWith(RectArea other)
        {
            return !(Right < other.Left || Left > other.Right ||
                     Bottom < other.Top || Top > other.Bottom);
        }

        public RectArea Intersection(RectArea other)
        {
            if (!IntersectsWith(other))
                return new RectArea(0, 0, 0, 0);

            int left = Math.Max(Left, other.Left);
            int top = Math.Max(Top, other.Top);
            int right = Math.Min(Right, other.Right);
            int bottom = Math.Min(Bottom, other.Bottom);

            return new RectArea(left, top, right - left, bottom - top);
        }

        public RectArea Union(RectArea other)
        {
            int left = Math.Min(Left, other.Left);
            int top = Math.Min(Top, other.Top);
            int right = Math.Max(Right, other.Right);
            int bottom = Math.Max(Bottom, other.Bottom);

            return new RectArea(left, top, right - left, bottom - top);
        }

        public void Inflate(int width, int height)
        {
            X -= width;
            Y -= height;
            Width += 2 * width;
            Height += 2 * height;
        }

        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public bool IsEmpty => Width == 0 || Height == 0;
    }
}

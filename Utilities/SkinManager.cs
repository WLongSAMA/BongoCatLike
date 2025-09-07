using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace BongoCat_Like.Utilities
{
    public class SkinManager
    {
        private static SkinManager instance = null!;
        private static readonly object obj = new();

        private ItemsJson items;
        private HatOffsetJson[] offset;
        private string _skinId = "0";
        private string _hatId = "0";
        private List<Bitmap> _skinImage = [];
        private Bitmap? _hatImage;
        private Position _hatOffset = new() { X = 30, Y = -70 };

        private SkinManager()
        {
            Stream itemsJson = AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/items.json"));
            items = JsonSerializer.Deserialize(itemsJson, ItemsJsonContext.Default.ItemsJson)!;

            Stream offsetJson = AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/offset.json"));
            offset = JsonSerializer.Deserialize(offsetJson, HatOffsetJsonContext.Default.HatOffsetJsonArray)!;

            DefaultImages();
        }

        public static SkinManager Instance
        {
            get
            {
                lock (obj)
                {
                    instance ??= new SkinManager();
                    return instance;
                }
            }
        }

        public ItemsJson Items { get => items; }

        public string SkinId
        {
            get => _skinId;
            set
            {
                _skinId = ValidateItem(value, Items.Skin);
                _skinImage.Clear();
                if (_skinId == "0")
                    DefaultImages();
                else
                    foreach (string img in Items.Skin[_skinId.ToString()].Image)
                        _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/skin/{img}"))));
            }
        }

        public string HatId
        {
            get => _hatId;
            set
            {
                _hatId = ValidateItem(value, Items.Hat);
                if (_hatId == "0")
                {
                    _hatImage = null;
                    _hatOffset = new() { X = 30, Y = -70 };
                }
                else
                {
                    HatItem hat = Items.Hat[_hatId.ToString()];
                    _hatImage = new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/hat/{hat.Image}")));
                    _hatOffset = GetHatOffsetByName(hat.Name);
                }
            }
        }

        public Bitmap[] SkinImage
        {
            get => [.. _skinImage];
        }

        public Bitmap? HatImage
        {
            get => _hatImage;
        }

        public Position HatOffset
        {
            get => _hatOffset;
        }

        private Position GetHatOffsetByName(string name)
        {
            HatOffsetJson hat = offset.FirstOrDefault(hat => hat.Name == name)!;
            return hat != null ? hat.Position : new Position { X = 30, Y = -70 };
        }

        public IImmutableSolidColorBrush GetQuality(string tags)
        {
            if (tags.Contains("quality:legendary"))
                return Brushes.Orange;
            else if (tags.Contains("quality:epic"))
                return Brushes.DarkMagenta;
            else if (tags.Contains("quality:rare"))
                return Brushes.DeepSkyBlue;
            else if (tags.Contains("quality:uncommon"))
                return Brushes.Lime;
            else if (tags.Contains("quality:common"))
                return Brushes.Silver;

            return Brushes.Transparent;
        }

        private static string ValidateItem<T>(string value, IReadOnlyDictionary<string, T>? itemDict)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";
            return itemDict?.ContainsKey(value) == true ? value : "0";
        }

        private void DefaultImages()
        {
            _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeft.png"))));
            _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeftPunch.png"))));
            _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRight.png"))));
            _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRightPunch.png"))));
        }

        public RectArea GetImageArea()
        {
            using SKBitmap baseBitmap = ToSKBitmap(_skinImage[0]);
            using SKBitmap twoBitmap = ToSKBitmap(_skinImage[1]);
            using SKBitmap threeBitmap = ToSKBitmap(_skinImage[2]);
            using SKBitmap fourBitmap = ToSKBitmap(_skinImage[3]);

            RectArea rectArea = new(0, 0, 0, 0);
            Position hatOffset = GlobalHelper.CatSkin.HatOffset;

            using SKBitmap hatBitmap = _hatImage != null ? ToSKBitmap(_hatImage) : null!;

            if (hatBitmap != null)
            {
                rectArea.X = Math.Min(0, hatOffset.X);
                rectArea.Y = Math.Min(0, hatOffset.Y);
                rectArea.Right = Math.Max(baseBitmap.Width, hatOffset.X + hatBitmap.Width);
                rectArea.Bottom = Math.Max(baseBitmap.Height, hatOffset.Y + hatBitmap.Height);
                rectArea.Width = rectArea.Right - rectArea.Left;
                rectArea.Height = rectArea.Bottom - rectArea.Top;
            }
            else
            {
                rectArea.X = 0;
                rectArea.Y = 0;
                rectArea.Width = baseBitmap.Width;
                rectArea.Height = baseBitmap.Height;
            }

            SKImageInfo info = new(rectArea.Width, rectArea.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            int offsetX = -rectArea.Left;
            int offsetY = -rectArea.Top;
            canvas.DrawBitmap(baseBitmap, offsetX, offsetY);
            canvas.DrawBitmap(twoBitmap, offsetX, offsetY);
            canvas.DrawBitmap(threeBitmap, offsetX, offsetY);
            canvas.DrawBitmap(fourBitmap, offsetX, offsetY);

            if (hatBitmap != null)
                canvas.DrawBitmap(hatBitmap, offsetX + hatOffset.X, offsetY + hatOffset.Y);

            using SKImage image = surface.Snapshot();
            using SKBitmap bitmap = SKBitmap.FromImage(image);

            RectArea imageArea = new(bitmap.Width, bitmap.Height, 0, 0);
            bool foundOpaquePixel = false;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).Alpha != 0)
                    {
                        if (!foundOpaquePixel)
                        {
                            imageArea.X = x;
                            imageArea.Y = y;
                            imageArea.Width = x;
                            imageArea.Height = y;
                            foundOpaquePixel = true;
                        }
                        else
                        {
                            imageArea.X = Math.Min(imageArea.X, x);
                            imageArea.Width = Math.Max(imageArea.Width, x);
                            imageArea.Y = Math.Min(imageArea.Y, y);
                            imageArea.Height = Math.Max(imageArea.Height, y);
                        }
                    }
                }
            }

            if (!foundOpaquePixel)
                return new RectArea(0, 0, 0, 0);

            imageArea.Width = imageArea.Width - imageArea.X + 1;
            imageArea.Height = imageArea.Height - imageArea.Y + 1;

            return imageArea;
        }

        private static SKBitmap ToSKBitmap(Bitmap bitmap)
        {
            try
            {
                using MemoryStream memoryStream = new();
                bitmap.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return SKBitmap.Decode(memoryStream);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to convert Bitmap to SKBitmap via stream", ex);
            }
        }
    }

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

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

        public static IImmutableSolidColorBrush GetQuality(string tags)
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

        public string[] GetSkinIdList()
        {
            List<string> list = [];
            foreach (KeyValuePair<string, SkinItem> skinItem in items.Skin)
                list.Add(skinItem.Key);
            return [.. list];
        }

        public string[] GetHatIdList()
        {
            List<string> list = [];
            foreach (KeyValuePair<string, HatItem> hatItem in items.Hat)
                list.Add(hatItem.Key);
            return [.. list];
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
}

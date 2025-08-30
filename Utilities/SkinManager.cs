using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
        private Position? _hatOffset = new() { X = 30, Y = -70 };

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

        public Position? HatOffset
        {
            get => _hatOffset;
        }

        private Position? GetHatOffsetByName(string name)
        {
            return offset.FirstOrDefault(hat => hat.Name == name)?.Position;
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
    }
}

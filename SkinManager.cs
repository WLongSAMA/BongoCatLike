using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BongoCat_Like
{
    public class SkinManager
    {
        private static SkinManager instance = null!;
        private static readonly object obj = new();
        private static ItemsJson Items = new();

        private string _skinId = "0";
        private string _hatId = "0";
        private List<Bitmap> _skinImage = [];
        private Bitmap? _hatImage;

        private SkinManager()
        {
            Stream json = AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/items.json"));
            using StreamReader streamReader = new(json);
            Items = JsonSerializer.Deserialize(streamReader.ReadToEnd(), ItemsJsonContext.Default.ItemsJson)!;
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

        public string SkinId
        {
            get => _skinId;
            set
            {
                _skinId = ValidateItem(value, Items?.Skin);
                _skinImage.Clear();
                if (_skinId == "0")
                {
                    DefaultImages();
                }
                else
                {
                    foreach (string img in Items?.Skin![_skinId.ToString()].Image!)
                    {
                        _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/skin/{img}"))));
                    }
                }
            }
        }

        public string HatId
        {
            get => _hatId;
            set
            {
                _hatId = ValidateItem(value, Items?.Hat);
                if (_hatId == "0")
                {
                    _hatImage = null!;
                }
                else
                {
                    _hatImage = new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/hat/{Items?.Hat![_hatId.ToString()].Image}")));
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

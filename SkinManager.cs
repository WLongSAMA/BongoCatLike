using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BongoCat_Like
{
    public class SkinManager
    {
        private static SkinManager instance = null!;
        private static readonly object obj = new();

        private int _skinId = 0;
        private int _hatId = 0;
        private List<Bitmap> _skinImage = [
            new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeft.png"))),
            new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeftPunch.png"))),
            new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRight.png"))),
            new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRightPunch.png")))
        ];
        private Bitmap? _hatImage;

        private SkinManager() { }

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

        public int SkinId
        {
            get => _skinId;
            set
            {
                _skinId = ValidateItem(value, GlobalHelper.Items?.Skin);
                _skinImage.Clear();
                if (_skinId == 0)
                {
                    _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeft.png"))));
                    _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatLeftPunch.png"))));
                    _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRight.png"))));
                    _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/default/CatRightPunch.png"))));
                }
                else
                {
                    foreach (string img in GlobalHelper.Items?.Skin![_skinId.ToString()].Image!)
                    {
                        _skinImage.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/skin/{img}"))));
                    }
                }
            }
        }

        public int HatId
        {
            get => _hatId;
            set
            {
                _hatId = ValidateItem(value, GlobalHelper.Items?.Hat);
                if (_hatId == 0)
                {
                    _hatImage = null!;
                }
                else
                {
                    _hatImage = new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/hat/{GlobalHelper.Items?.Hat![_hatId.ToString()].Image}")));
                }
            }
        }

        private static int ValidateItem<T>(int value, IReadOnlyDictionary<string, T>? itemDict)
        {
            return itemDict?.ContainsKey(value.ToString()) == true ? value : 0;
        }

        public Bitmap[] SkinImage
        {
            get => [.. _skinImage];
        }

        public Bitmap? HatImage
        {
            get => _hatImage;
        }
    }
}

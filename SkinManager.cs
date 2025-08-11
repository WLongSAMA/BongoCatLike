using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BongoCat_Like
{
    public class SkinManager
    {
        private int _skinId;
        private int _hatId;
        private List<Bitmap>? _skinImage;
        private Bitmap? _hatImage;

        public int SkinId
        {
            get => _skinId;
            set
            {
                _skinId = ValidateItem(value, GlobalHelper.Items?.Skin);
                if (_skinId == 0)
                {
                    _skinImage = [];
                }
                else
                {
                    _skinImage = [];
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
            get => [.. _skinImage!];
        }

        public Bitmap HatImage
        {
            get => _hatImage!;
        }
    }
}

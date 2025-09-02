using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BongoCat_Like.Utilities
{
    public class GlobalHelper
    {
        public static string ProjectName { get; } = "BongoCat_Like";
        public static string Name { get; } = "BongoCat-Like";
        public static string Version { get; } = "1.0.20250902";
        public static string Url { get; } = "https://github.com/WLongSAMA/BongoCatLike";
        public static int WindowWidth { get; } = 350;
        public static int WindowHeight { get; } = 350;
        public static SkinManager CatSkin { get; set; } = SkinManager.Instance;
        public static AppConfig Config = ConfigManager.LoadConfig();

        public static double GetScaling(int index)
        {
            return index switch
            {
                0 => 0.25,
                1 => 0.375,
                2 => 0.5,
                3 => 0.625,
                4 => 0.75,
                5 => 1,
                6 => 1.1,
                _ => 1
            };
        }
    }

    public class ItemsJson
    {
        [JsonPropertyName("skin")]
        public required Dictionary<string, SkinItem> Skin { get; set; }

        [JsonPropertyName("hat")]
        public required Dictionary<string, HatItem> Hat { get; set; }
    }

    public class ItemBase
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("modified")]
        public required string Modified { get; set; }

        [JsonPropertyName("created")]
        public required string Created { get; set; }

        [JsonPropertyName("icon")]
        public required string Icon { get; set; }

        [JsonPropertyName("tags")]
        public required string Tags { get; set; }
    }

    public class SkinItem : ItemBase
    {
        [JsonPropertyName("image")]
        public required string[] Image { get; set; }
    }

    public class HatItem : ItemBase
    {
        [JsonPropertyName("image")]
        public required string Image { get; set; }
    }

    [JsonSerializable(typeof(ItemsJson))]
    public partial class ItemsJsonContext : JsonSerializerContext { }

    public class HatOffsetJson
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("position")]
        public required Position Position { get; set; }
    }

    public class Position
    {
        [JsonPropertyName("x")]
        public int X { get; set; }
        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    [JsonSerializable(typeof(HatOffsetJson[]))]
    public partial class HatOffsetJsonContext : JsonSerializerContext { }
}

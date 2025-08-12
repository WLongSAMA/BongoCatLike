using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BongoCat_Like
{
    public class GlobalHelper
    {
        public static string ProjectName { get; set; } = "BongoCat_Like";
        public static string Name { get; set; } = "BongoCat-Like";
        public static int WindowWidth { get; } = 350;
        public static int WindowHeight { get; } = 350;
        public static ItemsJson? Items { get; set; }
        public static SkinManager CatSkin { get; set; } = SkinManager.Instance;
    }

    public class ItemsJson
    {
        [JsonPropertyName("skin")]
        public Dictionary<string, SkinItem>? Skin { get; set; }

        [JsonPropertyName("hat")]
        public Dictionary<string, HatItem>? Hat { get; set; }
    }

    public class ItemBase
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("Timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("modified")]
        public string? Modified { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("name_color")]
        public string? NameColor { get; set; }

        [JsonPropertyName("tags")]
        public string? Tags { get; set; }
    }

    public class SkinItem : ItemBase
    {
        [JsonPropertyName("image")]
        public string[]? Image { get; set; }
    }

    public class HatItem : ItemBase
    {
        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }
}

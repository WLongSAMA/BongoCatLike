using System.ComponentModel;

namespace BongoCat_Like
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static string DefaultSkinText => Localization.Instance["SettingWindow.DefaultSkin.Header"];
        public static string CustomSkinText => Localization.Instance["SettingWindow.CustomSkin.Header"];
        public static string SettingText => Localization.Instance["SettingWindow.Setting.Header"];
        public static string SystemSettingText => Localization.Instance["SettingWindow.Setting.Setting"];
        public static string LanguageText => Localization.Instance["SettingWindow.Setting.Language"];
        public static string TopmostText => Localization.Instance["SettingWindow.Setting.Topmost"];
        public static string AutorunText => Localization.Instance["SettingWindow.Setting.Autorun"];
        public static string TaskBarText => Localization.Instance["SettingWindow.Setting.TaskBar"];
        public static string VisualsText => Localization.Instance["SettingWindow.Setting.Visuals"];
        public static string FlipText => Localization.Instance["SettingWindow.Setting.Flip"];
        public static string ZoomText => Localization.Instance["SettingWindow.Setting.Zoom"];
        public static string InteractionText => Localization.Instance["SettingWindow.Setting.Interaction"];
        public static string DisableDragText => Localization.Instance["SettingWindow.Setting.DisableDrag"];
        public static string AdsorptionText => Localization.Instance["SettingWindow.Setting.Adsorption"];
        public static string RandomText => Localization.Instance["SettingWindow.Setting.Random"];
        public static string RandomSkinText => Localization.Instance["SettingWindow.Setting.RandomSkin"];
        public static string NeverText => Localization.Instance["SettingWindow.Setting.Never"];
        public static string MinuteText => Localization.Instance["SettingWindow.Setting.Minute"];
        public static string MinutesText => Localization.Instance["SettingWindow.Setting.Minutes"];
        public static string ExitAppText => Localization.Instance["SettingWindow.Setting.ExitApp"];
        public static string ExitText => Localization.Instance["SettingWindow.Setting.Exit"];

        public SettingViewModel()
        {
            Localization.LanguageChanged += () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultSkinText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomSkinText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SettingText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemSettingText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LanguageText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopmostText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutorunText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskBarText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisualsText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlipText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractionText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisableDragText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdsorptionText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RandomText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RandomSkinText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NeverText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinuteText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinutesText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExitAppText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExitText)));
            };
        }
    }
}

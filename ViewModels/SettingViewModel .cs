using System.ComponentModel;
using BongoCat_Like.Utilities;

namespace BongoCat_Like.ViewModels
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static string DefaultSkinText => Localization.Instance["SettingWindow.DefaultSkin.Header"];
        public static string CustomSkinText => Localization.Instance["SettingWindow.CustomSkin.Header"];
        public static string EmptySkinText => Localization.Instance["SettingWindow.CustomSkin.EmptySkin"];
        public static string SettingText => Localization.Instance["SettingWindow.Setting.Header"];
        public static string SystemSettingText => Localization.Instance["SettingWindow.Setting.Setting"];
        public static string LanguageText => Localization.Instance["SettingWindow.Setting.Language"];
        public static string TopmostText => Localization.Instance["SettingWindow.Setting.Topmost"];
        public static string AutorunText => Localization.Instance["SettingWindow.Setting.Autorun"];
        public static string TaskbarIconText => Localization.Instance["SettingWindow.Setting.TaskbarIcon"];
        public static string VisualsText => Localization.Instance["SettingWindow.Setting.Visuals"];
        public static string FlipText => Localization.Instance["SettingWindow.Setting.Flip"];
        public static string ZoomText => Localization.Instance["SettingWindow.Setting.Zoom"];
        public static string InteractionText => Localization.Instance["SettingWindow.Setting.Interaction"];
        public static string DisableDragText => Localization.Instance["SettingWindow.Setting.DisableDrag"];
        public static string AdsorptionText => Localization.Instance["SettingWindow.Setting.Adsorption"];
        public static string RandomText => Localization.Instance["SettingWindow.Setting.Random"];
        public static string RandomSkinText => Localization.Instance["SettingWindow.Setting.RandomSkin"];
        public static string NeverText => Localization.Instance["SettingWindow.Setting.Never"];
        public static string OneMinuteText => Localization.Instance["SettingWindow.Setting.OneMinute"];
        public static string ThreeMinutesText => Localization.Instance["SettingWindow.Setting.ThreeMinutes"];
        public static string FiveMinutesText => Localization.Instance["SettingWindow.Setting.FiveMinutes"];
        public static string FifteenMinutesText => Localization.Instance["SettingWindow.Setting.FifteenMinutes"];
        public static string ThirtyMinutesText => Localization.Instance["SettingWindow.Setting.ThirtyMinutes"];
        public static string VersionText => Localization.Instance["SettingWindow.Setting.Version"];
        public static string HomeText => Localization.Instance["SettingWindow.Setting.Home"];
        public static string UpdateText => Localization.Instance["SettingWindow.Setting.Update"];
        public static string ExitAppText => Localization.Instance["SettingWindow.Setting.ExitApp"];
        public static string ExitText => Localization.Instance["SettingWindow.Setting.Exit"];

        public static bool TopmostValue => GlobalHelper.Config.Topmost;
        public static bool AutorunValue => GlobalHelper.Config.Autorun;
        public static bool TaskbarIconValue => GlobalHelper.Config.TaskbarIcon;
        public static bool FlipValue => GlobalHelper.Config.Flip;
        public static int ZoomValue => GlobalHelper.Config.Zoom;
        public static bool DisableDragValue => GlobalHelper.Config.DisableDrag;
        public static bool AdsorptionValue => GlobalHelper.Config.Adsorption;

        public static int RandomSkinValue => GlobalHelper.Config.RandomSkin;

        public SettingViewModel()
        {
            Localization.LanguageChanged += () =>
            {
                OnPropertyChanged(nameof(DefaultSkinText));
                OnPropertyChanged(nameof(CustomSkinText));
                OnPropertyChanged(nameof(EmptySkinText));
                OnPropertyChanged(nameof(SettingText));
                OnPropertyChanged(nameof(SystemSettingText));
                OnPropertyChanged(nameof(LanguageText));
                OnPropertyChanged(nameof(TopmostText));
                OnPropertyChanged(nameof(AutorunText));
                OnPropertyChanged(nameof(TaskbarIconText));
                OnPropertyChanged(nameof(VisualsText));
                OnPropertyChanged(nameof(FlipText));
                OnPropertyChanged(nameof(ZoomText));
                OnPropertyChanged(nameof(InteractionText));
                OnPropertyChanged(nameof(DisableDragText));
                OnPropertyChanged(nameof(AdsorptionText));
                OnPropertyChanged(nameof(RandomText));
                OnPropertyChanged(nameof(RandomSkinText));
                OnPropertyChanged(nameof(NeverText));
                OnPropertyChanged(nameof(OneMinuteText));
                OnPropertyChanged(nameof(ThreeMinutesText));
                OnPropertyChanged(nameof(FiveMinutesText));
                OnPropertyChanged(nameof(FifteenMinutesText));
                OnPropertyChanged(nameof(ThirtyMinutesText));
                OnPropertyChanged(nameof(VersionText));
                OnPropertyChanged(nameof(HomeText));
                OnPropertyChanged(nameof(UpdateText));
                OnPropertyChanged(nameof(ExitAppText));
                OnPropertyChanged(nameof(ExitText));
            };
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

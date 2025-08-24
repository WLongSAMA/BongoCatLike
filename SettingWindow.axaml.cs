using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BongoCat_Like;

public partial class SettingWindow : Window
{
    public static SettingWindow? CurrentInstance { get; private set; }

    private Dictionary<string, string> LangList = [];

    public SettingWindow()
    {
        InitializeComponent();
        DataContext = new SettingViewModel();
        Closed += (s, e) =>
        {
            CurrentInstance = null;
        };

        foreach (KeyValuePair<string, SkinItem> skinItem in GlobalHelper.CatSkin.Items.Skin!)
        {
            Image image = new()
            {
                Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/skin/{skinItem.Value.Icon}"))),
                Width = 50,
                Height = 50
            };

            Border border = new()
            {
                Tag = new
                {
                    Id = skinItem.Key,
                    skinItem.Value.Name,
                    Type = "skin"
                },
                Child = image,
                BorderBrush = GlobalHelper.CatSkin.GetQuality(skinItem.Value.Tags!),
                BorderThickness = new Thickness(5),
                Margin = new Thickness(0, 0, 5, 5)
            };

            border.PointerPressed += Border_PointerPressed;
            DefaultSkinList.Children.Add(border);
        }

        foreach (KeyValuePair<string, HatItem> hatItem in GlobalHelper.CatSkin.Items.Hat!)
        {
            Image image = new()
            {
                Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/hat/{hatItem.Value.Icon}"))),
                Width = 50,
                Height = 50
            };

            Border border = new()
            {
                Tag = new
                {
                    Id = hatItem.Key,
                    hatItem.Value.Name,
                    Type = "hat"
                },
                Child = image,
                BorderBrush = GlobalHelper.CatSkin.GetQuality(hatItem.Value.Tags!),
                BorderThickness = new Thickness(5),
                Margin = new Thickness(0, 0, 5, 5)
            };

            border.PointerPressed += Border_PointerPressed;
            DefaultSkinList.Children.Add(border);
        }

        LangList = Localization.GetLangList();
        foreach (KeyValuePair<string, string> lang in LangList)
        {
            LangListComboBox.Items.Add(lang.Value);
            if (Localization.GetSystemLang() == lang.Key)
                LangListComboBox.SelectedIndex = LangListComboBox.Items.Count - 1;
        }
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (border.Tag != null)
        {
            var tagType = border.Tag.GetType();
            var typeProperty = tagType.GetProperty("Type");
            var type = typeProperty!.GetValue(border.Tag) as string;
            var idProperty = tagType.GetProperty("Id");
            var id = idProperty!.GetValue(border.Tag) as string;
            if (type!.Equals("skin"))
                App.MainWindow!.SetSkin(id!);
            else if (type!.Equals("hat"))
                App.MainWindow!.SetHat(id!);
        }
    }

    public void NavigateToPage(string pageName)
    {
        if (NavigationTabs == null)
            return;

        NavigationTabs.SelectedIndex = pageName switch
        {
            "Page1" => 0,
            "Page2" => 1,
            "Setting" => 2,
            _ => 0,
        };
    }

    public static void ShowOrActivate(Window owner, string? navigateTo = null)
    {
        if (CurrentInstance != null && !CurrentInstance.IsClosed)
        {
            CurrentInstance.Activate();
            CurrentInstance.Topmost = true;
            CurrentInstance.Topmost = false;

            if (!string.IsNullOrEmpty(navigateTo))
            {
                CurrentInstance.NavigateToPage(navigateTo);
            }
            return;
        }

        CurrentInstance = new SettingWindow
        {
            Owner = owner,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (!string.IsNullOrEmpty(navigateTo))
        {
            CurrentInstance.NavigateToPage(navigateTo);
        }

        CurrentInstance.Show();
    }

    private bool _isClosed = false;
    public bool IsClosed => _isClosed;

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _isClosed = true;
    }

    private void OnLangListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem;
            if (selectedItem != null)
            {
                foreach (KeyValuePair<string, string> lang in LangList)
                {
                    if (lang.Value == (string)selectedItem)
                    {
                        Localization.LoadLanguage(lang.Key);
                        GlobalHelper.Config.Language = lang.Key;

                        //解决切换语言后，组合框显示内容不变的问题
                        //RandomSkinComboBox.SelectedIndex = -1;
                        //RandomSkinComboBox.SelectedIndex = GlobalHelper.Config.RandomSkin;
                    }
                }
            }
        }
    }

    private void OnTopmostClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            App.MainWindow!.Topmost = GlobalHelper.Config.Topmost = checkBox.IsChecked.GetValueOrDefault();
        }
    }

    private void OnAutorunClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            GlobalHelper.Config.Autorun = checkBox.IsChecked.GetValueOrDefault();
            try
            {
                if (GlobalHelper.Config.Autorun)
                {
                    if (Environment.ProcessPath is string file)
                        AutorunManager.Add(GlobalHelper.Name, file);
                }
                else
                {
                    AutorunManager.Remove(GlobalHelper.Name);
                }
            }
            catch { }
        }
    }

    private void OnTaskbarIconClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            App.MainWindow!.ShowInTaskbar = GlobalHelper.Config.TaskbarIcon = checkBox.IsChecked.GetValueOrDefault();
        }
    }

    private void OnFlipClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            GlobalHelper.Config.Flip = checkBox.IsChecked.GetValueOrDefault();
            App.MainWindow!.SetFlip(GlobalHelper.Config.Flip);
        }
    }

    private void OnZoomSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            GlobalHelper.Config.Zoom = comboBox.SelectedIndex;
            App.MainWindow!.SetZoom(comboBox.SelectedIndex);
        }
    }

    private void OnDisableDragClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            GlobalHelper.Config.DisableDrag = checkBox.IsChecked.GetValueOrDefault();
        }
    }

    private void OnAdsorptionClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            GlobalHelper.Config.Adsorption = checkBox.IsChecked.GetValueOrDefault();
        }
    }

    private void OnRandomSkinSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            GlobalHelper.Config.RandomSkin = comboBox.SelectedIndex;
            App.MainWindow!.RandomSkin(comboBox.SelectedIndex);
        }
    }

    private void OnHomeClicked(object? sender, RoutedEventArgs e)
    {
        Launcher.LaunchUriAsync(new Uri(GlobalHelper.Url));
    }

    private void OnUpdateClicked(object? sender, RoutedEventArgs e)
    {
        Launcher.LaunchUriAsync(new Uri(GlobalHelper.Url + "/releases/latest"));
    }

    private void OnExitClicked(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}

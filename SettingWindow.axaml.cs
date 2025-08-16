using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace BongoCat_Like;

public partial class SettingWindow : Window
{
    public static SettingWindow? CurrentInstance { get; private set; }

    public SettingWindow()
    {
        InitializeComponent();
        DataContext = new SettingViewModel();
        Closed += (s, e) =>
        {
            CurrentInstance = null;
        };

        foreach (KeyValuePair<string, string> lang in Localization.GetLangList())
        {
            LangList.Items.Add(lang.Value);
            if (Localization.GetSystemLang() == lang.Key)
                LangList.SelectedIndex = LangList.Items.Count - 1;
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
}

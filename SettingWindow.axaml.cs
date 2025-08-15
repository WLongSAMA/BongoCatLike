using System;
using Avalonia.Controls;

namespace BongoCat_Like;

public partial class SettingWindow : Window
{
    public static SettingWindow? CurrentInstance { get; private set; }

    public SettingWindow()
    {
        InitializeComponent();

        Closed += (s, e) =>
        {
            CurrentInstance = null;
        };
    }

    public void NavigateToPage(string pageName)
    {
        if (NavigationTabs == null)
            return;

        switch (pageName.ToLower())
        {
            case "Page1":
                NavigationTabs.SelectedIndex = 0;
                break;
            case "Page2":
                NavigationTabs.SelectedIndex = 1;
                break;
            case "Setting":
                NavigationTabs.SelectedIndex = 2;
                break;
            default:
                NavigationTabs.SelectedIndex = 0;
                break;
        }
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

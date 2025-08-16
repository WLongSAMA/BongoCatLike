using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using SharpHook;
using SharpHook.Data;
using MouseButton = SharpHook.Data.MouseButton;

namespace BongoCat_Like
{
    public partial class MainWindow : Window
    {
        private TrayIcon _trayIcon = new();
        private NativeMenuItem? _skinItem;
        private NativeMenuItem? _settingItem;
        private NativeMenuItem? _exitItem;
        private bool Hand = false;
        private bool animationLock = false;

        private ConcurrentDictionary<KeyCode, bool> _activeKeys = new();
        private ConcurrentDictionary<MouseButton, bool> _activeMouseButtons = new();

        public MainWindow()
        {
            InitializeComponent();
            Opened += MainWindow_Opened;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Opened(object? sender, EventArgs e)
        {
            ReadAssets();
            SetLocalization();
            SetTrayIcon();
            SetWindow();
            SetSkin();
            SetHat();
            ListeningPress();
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            GlobalHelper._config.WindowLeft = Position.X;
            GlobalHelper._config.WindowTop = Position.Y;
            GlobalHelper._config.SkinId = GlobalHelper.CatSkin.SkinId;
            GlobalHelper._config.HatId = GlobalHelper.CatSkin.HatId;
            ConfigManager.SaveConfig(GlobalHelper._config);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
            base.OnPointerPressed(e);
        }

        private void Exit(object? sender, RoutedEventArgs e)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        }

        private static void ReadAssets()
        {
            Stream json = AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/items.json"));
            using StreamReader streamReader = new(json);
            GlobalHelper.Items = JsonSerializer.Deserialize(streamReader.ReadToEnd(), ItemsJsonContext.Default.ItemsJson);
        }

        private void SetLocalization()
        {
            Localization.LoadLanguage(GlobalHelper._config.Language);
            Localization.LanguageChanged += () =>
            {
                if (_skinItem != null)
                    _skinItem.Header = Localization.GetString("Menu.Skin");
                if (_settingItem != null)
                    _settingItem.Header = Localization.GetString("Menu.Setting");
                if (_exitItem != null)
                    _exitItem.Header = Localization.GetString("Menu.Exit");
            };
        }

        private void SetTrayIcon()
        {
            Bitmap icon = new(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/Icon.ico")));
            _trayIcon.Icon = new WindowIcon(icon);
            _trayIcon.ToolTipText = GlobalHelper.Name;

            _skinItem = new(Localization.GetString("Menu.Skin"));
            _skinItem.Click += (sender, e) => { SettingWindow.ShowOrActivate(this, "Page1"); };

            _settingItem = new(Localization.GetString("Menu.Setting"));
            _settingItem.Click += (sender, e) => { SettingWindow.ShowOrActivate(this, "Setting"); };

            _exitItem = new(Localization.GetString("Menu.Exit"));
            _exitItem.Click += (sender, e) =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    desktop.Shutdown();
            };

            NativeMenu menu = [];
            menu.Add(_skinItem);
            menu.Add(_settingItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(_exitItem);

            _trayIcon.Menu = menu;

            TrayIcon.SetIcons(Application.Current!, [_trayIcon]);
        }

        private void SetWindow()
        {
            if (GlobalHelper._config.WindowLeft == 0 && GlobalHelper._config.WindowTop == 0)
            {
                Screen screen = Screens.Primary!;
                if (screen == null)
                    return;

                double scaling = screen.Scaling;
                PixelRect workingArea = screen.WorkingArea;

                // 获取窗口的物理像素尺寸
                Size pixelSize = FrameSize ?? ClientSize * scaling;

                double left = workingArea.X + (workingArea.Width - pixelSize.Width) / 2;
                double top = workingArea.Y + (workingArea.Height - pixelSize.Height) / 2;

                // 确保位置在屏幕范围内
                GlobalHelper._config.WindowLeft = (int)Math.Max(workingArea.X, Math.Min(left, workingArea.X + workingArea.Width - pixelSize.Width));
                GlobalHelper._config.WindowTop = (int)Math.Max(workingArea.Y, Math.Min(top, workingArea.Y + workingArea.Height - pixelSize.Height));
            }
            Position = new PixelPoint(GlobalHelper._config.WindowLeft, GlobalHelper._config.WindowTop);

            MainGrid.Margin = new Thickness(GlobalHelper._config.MainOffsetX, GlobalHelper._config.MainOffsetY);
            ShowInTaskbar = GlobalHelper._config.TaskbarIcon;
            Topmost = GlobalHelper._config.Topmost;

            SetScale(GlobalHelper._config.Scale);
        }

        private void ListeningPress()
        {
            Task task = Task.Run(() =>
            {
                TaskPoolGlobalHook hook = new();

                hook.KeyPressed += async (sender, e) =>
                {
                    if (!_activeKeys.ContainsKey(e.Data.KeyCode))
                    {
                        _activeKeys[e.Data.KeyCode] = true;
                        await Hit();
                    }
                };

                hook.KeyReleased += (sender, e) =>
                {
                    _activeKeys.TryRemove(e.Data.KeyCode, out _);
                };

                hook.MousePressed += async (sender, e) =>
                {
                    if (!_activeMouseButtons.ContainsKey(e.Data.Button))
                    {
                        _activeMouseButtons[e.Data.Button] = true;
                        await Hit();
                    }
                };

                hook.MouseReleased += (sender, e) =>
                {
                    _activeMouseButtons.TryRemove(e.Data.Button, out _);
                };

                hook.Run();
            });
        }

        private async Task Hit()
        {
            if (animationLock)
                return;

            animationLock = true;

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (Hand)
                {
                    SkinImage.Source = GlobalHelper.CatSkin.SkinImage[1];
                    HandImage.Source = GlobalHelper.CatSkin.SkinImage[2];
                }
                else
                {
                    SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
                    HandImage.Source = GlobalHelper.CatSkin.SkinImage[3];
                }

                await Task.Delay(200);

                SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
                HandImage.Source = GlobalHelper.CatSkin.SkinImage[2];
                Hand = !Hand;
            });

            animationLock = false;
        }

        private void ChangeSkin(object sender, RoutedEventArgs e)
        {
            GlobalHelper._config.SkinId = 236;
            GlobalHelper._config.HatId = 432;
            SetSkin();
            SetHat();
        }

        private void PlayAnimation(object sender, RoutedEventArgs e)
        {
            HatAnimation();
        }

        private void Scale(object sender, RoutedEventArgs e)
        {
            /*
             * Like    Cat
             * 0.25    0.5
             * 0.375   0.75
             * 0.5     1
             * 0.625   1.25
             * 0.75    1.5
             * 1       2
             * 1.11    2.22
             */
            SetScale(0.625);
        }

        private void SetScale(double scale)
        {
            TransformGroup transformGroup = new();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            MainGrid.RenderTransform = transformGroup;
            GlobalHelper._config.Scale = scale;

            Animation animation = new()
            {
                Duration = TimeSpan.FromSeconds(1),
                IterationCount = IterationCount.Infinite,
                Easing = new Avalonia.Animation.Easings.QuadraticEaseIn(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.0 * scale) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.5),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.05 * scale) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.0 * scale) }
                    }
                }
            };
            animation.RunAsync(MainGrid);
        }

        private void SetSkin()
        {
            GlobalHelper.CatSkin.SkinId = GlobalHelper._config.SkinId;
            SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
            HandImage.Source = GlobalHelper.CatSkin.SkinImage[2];
        }

        private void SetHat()
        {
            GlobalHelper.CatSkin.HatId = GlobalHelper._config.HatId;
            HatImage.Source = GlobalHelper.CatSkin.HatImage;
        }

        private async void HatAnimation()
        {
            TransformGroup transformGroup = new();
            transformGroup.Children.Add(new ScaleTransform(1, 1));
            transformGroup.Children.Add(new TranslateTransform(32, -70));
            HatImage.RenderTransform = transformGroup;

            await Task.Delay(10);

            Animation animation = new()
            {
                Duration = TimeSpan.FromSeconds(0.5),
                Easing = new Avalonia.Animation.Easings.QuadraticEaseInOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 0.4),
                            new Setter(ScaleTransform.ScaleYProperty, 0.4)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.3),
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 1.1),
                            new Setter(ScaleTransform.ScaleYProperty, 1.1)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.5),
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 0.8),
                            new Setter(ScaleTransform.ScaleYProperty, 0.8)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.7),
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 1.2),
                            new Setter(ScaleTransform.ScaleYProperty, 1.2)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 1.0),
                            new Setter(ScaleTransform.ScaleYProperty, 1.0)
                        }
                    }
                }
            };
            await animation.RunAsync(HatImage);
        }
    }
}

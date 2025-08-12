using System;
using System.Diagnostics;
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
using SharpHook;

namespace BongoCat_Like
{
    public partial class MainWindow : Window
    {
        private AppConfig _config = new();
        private TrayIcon _trayIcon = null!;
        private NativeMenuItem? _showItem;
        private NativeMenuItem? _exitItem;

        public MainWindow()
        {
            InitializeComponent();
            Opened += MainWindow_Opened;
            Closing += MainWindow_Closing;
            PointerPressed += MainWindow_PointerPressed;
        }

        private void MainWindow_Opened(object? sender, EventArgs e)
        {
            ReadAssets();
            ReadConfig();
            SetLocalization();
            SetTrayIcon();
            SetPosition();
            //ListeningPress();
            SetSkin();
            SetHat();
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            _config.WindowLeft = Position.X;
            _config.WindowTop = Position.Y;
            ConfigManager.SaveConfig(_config);
        }

        private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
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
            GlobalHelper.Items = JsonSerializer.Deserialize<ItemsJson>(streamReader.ReadToEnd());
        }

        private void ReadConfig()
        {
            _config = ConfigManager.LoadConfig();
        }

        private void SetLocalization()
        {
            Localization.LoadLanguage(_config.Language);
            Localization.LanguageChanged += () =>
            {
                if (_showItem != null)
                    _showItem.Header = Localization.GetString("Menu.Setting");
                if (_exitItem != null)
                    _exitItem.Header = Localization.GetString("Menu.Exit");
            };
        }

        private void SetTrayIcon()
        {
            _trayIcon = new TrayIcon();
            Bitmap icon = new(AssetLoader.Open(new Uri($"avares://{GlobalHelper.ProjectName}/Assets/tray.ico")));
            _trayIcon.Icon = new WindowIcon(icon);
            _trayIcon.ToolTipText = GlobalHelper.Name;

            _showItem = new(Localization.GetString("Menu.Setting"));
            _showItem.Click += (sender, e) => { };

            _exitItem = new(Localization.GetString("Menu.Exit"));
            _exitItem.Click += (sender, e) =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    desktop.Shutdown();
            };

            NativeMenu menu = [];
            menu.Add(_showItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(_exitItem);

            _trayIcon.Menu = menu;

            TrayIcon.SetIcons(Application.Current!, [_trayIcon]);
        }

        private void SetPosition()
        {
            if (_config.WindowLeft == 0 && _config.WindowTop == 0)
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
                _config.WindowLeft = (int)Math.Max(workingArea.X, Math.Min(left, workingArea.X + workingArea.Width - pixelSize.Width));
                _config.WindowTop = (int)Math.Max(workingArea.Y, Math.Min(top, workingArea.Y + workingArea.Height - pixelSize.Height));
            }
            WindowStartupLocation = WindowStartupLocation.Manual;
            Position = new PixelPoint(_config.WindowLeft, _config.WindowTop);
        }

        private static void ListeningPress()
        {
            Task task = Task.Run(() =>
            {
                TaskPoolGlobalHook hook = new();
                hook.KeyPressed += (sender, e) =>
                {
                    Trace.WriteLine($"检测到键盘按下：{e.Data.KeyCode}");
                };
                hook.MousePressed += (sender, e) =>
                {
                    Trace.WriteLine($"检测到鼠标点击：{e.Data.X}, {e.Data.Y}");
                };
                hook.Run();
            });
        }

        private void PlayAnimation(object sender, RoutedEventArgs e)
        {
            HatAnimation();
        }

        private void SetSkin()
        {
            SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
            HandImage.Source = GlobalHelper.CatSkin.SkinImage[1];
        }

        private void SetHat()
        {
            HatImage.Source = GlobalHelper.CatSkin.HatImage;
        }

        private async void HatAnimation()
        {
            // 重置变换确保动画从头开始
            TransformGroup transformGroup = new();
            transformGroup.Children.Add(new ScaleTransform(1, 1));
            transformGroup.Children.Add(new TranslateTransform(32, -70));
            HatImage.RenderTransform = transformGroup;

            // 强制重绘
            await Task.Delay(10);

            // 手动启动动画
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

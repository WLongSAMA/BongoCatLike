using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using BongoCat_Like.Utilities;
using SharpHook;
using SharpHook.Data;
using MouseButton = SharpHook.Data.MouseButton;

namespace BongoCat_Like.Views
{
    public partial class MainWindow : Window
    {
        private TrayIcon _trayIcon = new();
        private NativeMenuItem? _skinItem;
        private NativeMenuItem? _settingItem;
        private NativeMenuItem? _exitItem;
        private bool Hand = false;
        private bool animationLock = false;
        private Task? _bobbingAnimationInstance;
        private double LastHeight = 0;

        private ConcurrentDictionary<KeyCode, bool> _activeKeys = new();
        private ConcurrentDictionary<MouseButton, bool> _activeMouseButtons = new();

        DispatcherTimer RandomSkinTimer = new();
        private bool isRandomSkinTimerRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            Opened += MainWindow_Opened;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Opened(object? sender, EventArgs e)
        {
            SetLocalization();
            SetTrayIcon();
            SetSkin(GlobalHelper.Config.SkinId);
            SetHat(GlobalHelper.Config.HatId);
            SetWindow();
            SetRandomSkin();
            ListeningPress();
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            GlobalHelper.Config.WindowLeft = Position.X;
            GlobalHelper.Config.WindowTop = Position.Y;
            GlobalHelper.Config.SkinId = GlobalHelper.CatSkin.SkinId;
            GlobalHelper.Config.HatId = GlobalHelper.CatSkin.HatId;
            ConfigManager.SaveConfig(GlobalHelper.Config);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!GlobalHelper.Config.DisableDrag && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                BeginMoveDrag(e);
            base.OnPointerPressed(e);
        }

        private void SetLocalization()
        {
            Localization.LoadLanguage(GlobalHelper.Config.Language);
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
            _trayIcon.Clicked += (sender, e) =>
            {
                Show();
                Activate();
            };

            TrayIcon.SetIcons(Application.Current!, [_trayIcon]);
        }

        private void SetWindow()
        {
            if (GlobalHelper.Config.WindowLeft == 0 && GlobalHelper.Config.WindowTop == 0)
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
                GlobalHelper.Config.WindowLeft = (int)Math.Max(workingArea.X, Math.Min(left, workingArea.X + workingArea.Width - pixelSize.Width));
                GlobalHelper.Config.WindowTop = (int)Math.Max(workingArea.Y, Math.Min(top, workingArea.Y + workingArea.Height - pixelSize.Height));
            }
            Position = new PixelPoint(GlobalHelper.Config.WindowLeft, GlobalHelper.Config.WindowTop);

            ShowInTaskbar = GlobalHelper.Config.TaskbarIcon;
            Topmost = GlobalHelper.Config.Topmost;

            SetFlip(GlobalHelper.Config.Flip);
            SetZoom(GlobalHelper.Config.Zoom);
            SetBobbing(GlobalHelper.Config.Bobbing);
        }

        private void SetWindowSize(int type = 0)
        {
            double scaling = GlobalHelper.GetScaling(GlobalHelper.Config.Zoom);
            RectArea imageArea = GlobalHelper.CatSkin.GetImageArea();
            Width = imageArea.Width * scaling;
            Height = imageArea.Height * scaling;
            MainGrid.Margin = new Thickness(-imageArea.X, -imageArea.Y);

            if (type == 1)
            {
                // 计算小猫位置过于复杂，暂时不处理更换小猫皮肤的情况
            }
            else if (type == 2)
            {
                if (LastHeight != 0 && LastHeight != Height)
                    Position = new PixelPoint(Position.X, Position.Y + (int)LastHeight - (int)Height);
            }
            LastHeight = Height;
        }

        private void SetRandomSkin()
        {
            RandomSkinTimer.Tick += (sender, e) =>
            {
                Random random = new();
                string[] skinList = GlobalHelper.CatSkin.GetSkinIdList();
                string[] hatList = GlobalHelper.CatSkin.GetHatIdList();
                SetSkin(skinList[random.Next(skinList.Length)]);
                SetHat(hatList[random.Next(hatList.Length)]);
            };
            RandomSkin(GlobalHelper.Config.RandomSkin);
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

        public async Task Hit()
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

                await Task.Delay(100);

                SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
                HandImage.Source = GlobalHelper.CatSkin.SkinImage[2];
                Hand = !Hand;
            });

            animationLock = false;
        }

        public void EnableMousePenetration(bool isEnable)
        {
            if (!OperatingSystem.IsWindows())
                return;

            IPlatformHandle platformHandle = TopLevel.GetTopLevel(this)?.TryGetPlatformHandle()!;
            if (platformHandle != null)
            {
                if (isEnable)
                    MousePenetration.Enable(platformHandle.Handle);
                else
                    MousePenetration.Disable(platformHandle.Handle);
            }
            GlobalHelper.Config.MousePenetration = isEnable;
        }

        public void SetFlip(bool isFlip)
        {
            double scaling = GlobalHelper.GetScaling(GlobalHelper.Config.Zoom);
            MainGrid.RenderTransform = new ScaleTransform(isFlip ? -1 * scaling : scaling, scaling);
            MainGrid.HorizontalAlignment = isFlip ? Avalonia.Layout.HorizontalAlignment.Right : Avalonia.Layout.HorizontalAlignment.Left;
            GlobalHelper.Config.Flip = isFlip;
        }

        public void SetZoom(int index)
        {
            double scaling = GlobalHelper.GetScaling(index);
            MainGrid.RenderTransform = new ScaleTransform(GlobalHelper.Config.Flip ? -1 * scaling : scaling, scaling);
            MainGrid.HorizontalAlignment = GlobalHelper.Config.Flip ? Avalonia.Layout.HorizontalAlignment.Right : Avalonia.Layout.HorizontalAlignment.Left;
            GlobalHelper.Config.Zoom = index;
            SetWindowSize();
        }

        public void SetBobbing(bool isEnable)
        {
            if (_bobbingAnimationInstance != null)
            {
                RenderTransform = null;
                _bobbingAnimationInstance = null;
            }

            if (!isEnable)
            {
                RenderTransform = null;
                return;
            }

            Animation animation = new()
            {
                Duration = TimeSpan.FromSeconds(1),
                IterationCount = IterationCount.Infinite,
                Easing = new QuadraticEaseIn(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.0) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.5),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.02) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters = { new Setter(ScaleTransform.ScaleYProperty, 1.0) }
                    }
                }
            };

            RenderTransformOrigin = new RelativePoint(0.5, 1, RelativeUnit.Relative);
            _bobbingAnimationInstance = animation.RunAsync(this);
        }

        public void SetSkin(string SkinId)
        {
            GlobalHelper.CatSkin.SkinId = SkinId;
            SkinImage.Source = GlobalHelper.CatSkin.SkinImage[0];
            HandImage.Source = GlobalHelper.CatSkin.SkinImage[2];
            SetWindowSize(1);
        }

        public void SetHat(string HatId)
        {
            GlobalHelper.CatSkin.HatId = HatId;
            HatImage.Source = GlobalHelper.CatSkin.HatImage;

            RectArea rectArea = new(0, 0, 0, 0);
            Position hatOffset = GlobalHelper.CatSkin.HatOffset;
            Bitmap skinBitmap = GlobalHelper.CatSkin.SkinImage[0];
            Bitmap? hatBitmap = GlobalHelper.CatSkin.HatImage;
            if (hatBitmap != null)
            {
                rectArea.X = Math.Min(0, hatOffset.X);
                rectArea.Y = Math.Min(0, hatOffset.Y);
                rectArea.Right = Math.Max(skinBitmap.PixelSize.Width, hatOffset.X + hatBitmap.PixelSize.Width);
                rectArea.Bottom = Math.Max(skinBitmap.PixelSize.Height, hatOffset.Y + hatBitmap.PixelSize.Height);
                rectArea.Width = rectArea.Right - rectArea.Left;
                rectArea.Height = rectArea.Bottom - rectArea.Top;
            }
            else
            {
                rectArea.X = 0;
                rectArea.Y = 0;
                rectArea.Width = (int)SkinImage.Bounds.Width;
                rectArea.Height = (int)SkinImage.Bounds.Height;
            }

            int offsetX = -rectArea.X;
            int offsetY = -rectArea.Y;

            HatImage.RenderTransform = new TransformGroup
            {
                Children = {
                    new ScaleTransform(1, 1), //没有此属性会导致帽子动画失效
                    new TranslateTransform(offsetX + hatOffset.X, offsetY + hatOffset.Y)
                }
            };
            SkinImage.RenderTransform = new TranslateTransform(offsetX, offsetY);
            HandImage.RenderTransform = new TranslateTransform(offsetX, offsetY);

            SetWindowSize(2);
        }

        public async void HatAnimation()
        {
            await Task.Delay(10);

            Animation animation = new()
            {
                Duration = TimeSpan.FromSeconds(0.5),
                Easing = new QuadraticEaseInOut(),
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

        public void RandomSkin(int timeIndex)
        {
            GlobalHelper.Config.RandomSkin = timeIndex;
            if (timeIndex != 0)
            {
                if (!isRandomSkinTimerRunning)
                {
                    switch (timeIndex)
                    {
                        case 1:
                            RandomSkinTimer.Interval = TimeSpan.FromSeconds(60);
                            break;
                        case 2:
                            RandomSkinTimer.Interval = TimeSpan.FromSeconds(180);
                            break;
                        case 3:
                            RandomSkinTimer.Interval = TimeSpan.FromSeconds(300);
                            break;
                        case 4:
                            RandomSkinTimer.Interval = TimeSpan.FromSeconds(900);
                            break;
                        case 5:
                            RandomSkinTimer.Interval = TimeSpan.FromSeconds(1800);
                            break;
                        default:
                            return;
                    }
                    RandomSkinTimer.Start();
                }
                isRandomSkinTimerRunning = true;
            }
            else
            {
                if (isRandomSkinTimerRunning)
                    RandomSkinTimer.Stop();
                isRandomSkinTimerRunning = false;
            }
        }

        public void TaskbarAdsorption(bool isAdsorption)
        {
            GlobalHelper.Config.Adsorption = isAdsorption;
        }
    }
}

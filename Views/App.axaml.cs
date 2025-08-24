using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace BongoCat_Like
{
    public partial class App : Application
    {
        public static MainWindow MainWindow { get; private set; } = new MainWindow();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}

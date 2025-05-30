using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace ProyectoEmpresa
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(windows =>
                        windows.OnWindowCreated(window =>
                        {
                            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                            appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
                        }));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
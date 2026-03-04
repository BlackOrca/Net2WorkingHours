using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Net2WorkingHours.Controls;
using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Services;
using Net2WorkingHours.Interfaces;
using Net2WorkingHours.Services;
using Net2WorkingHours.Windows;

namespace Net2WorkingHours;

public partial class App : Application
{
    private readonly IServiceProvider ServiceProvider;

    public App()
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {       
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IEmployeeTabsControlFactory, EmployeeTabsControlFactory>();
        services.AddSingleton<ICsvImportDialogService, CsvImportDialogService>();
        // Configure Logging
        // services.AddLogging();

        services.AddSingleton<ISettingsService, SettingsService>();                
        services.AddSingleton<IExcelExportService, ExcelExportService>();

        // Register Components
        services.AddSingleton<SettingsControl>();
        services.AddSingleton<IEmployeeService, EmployeeService>();
        services.AddSingleton<IWorkSummaryService, WorkSummaryService>();
        services.AddSingleton<WorkSummaryGrid>();
        services.AddSingleton<MainWindow>();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        // Dispose of services if needed
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }        

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        string message = $"Fehler: {e.Exception.Message}\n\n{e.Exception.StackTrace}";
        System.Windows.MessageBox.Show(message, "Unerwarteter Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        e.Handled = true;

        Clipboard.SetText(message);

        Application.Current.Shutdown();
    }
}


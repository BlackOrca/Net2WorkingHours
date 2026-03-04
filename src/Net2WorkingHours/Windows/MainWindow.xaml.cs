using System.Windows;
using System.Windows.Controls;
using Net2WorkingHours.Controls;
using Net2WorkingHours.Core.Interfaces;
using Net2WorkingHours.Core.Models;
using Net2WorkingHours.Interfaces;

namespace Net2WorkingHours.Windows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Employee> Employees = [];
    private readonly IExcelExportService ExcelExportService;
    private readonly SettingsControl SettingsControl;
    private readonly IEmployeeService EmployeeService;
    private readonly ICsvImportDialogService CsvImportDialogService;
    private readonly IEmployeeTabsControlFactory EmployeeTabsControlFactory;

    public MainWindow(
        IExcelExportService excelExportService,
        SettingsControl settingsControl,
        IEmployeeService employeeService,
        ICsvImportDialogService csvImportDialogService,
        IEmployeeTabsControlFactory employeeTabsControlFactory)
    {
        InitializeComponent();
        ExcelExportService = excelExportService;
        SettingsControl = settingsControl;
        EmployeeService = employeeService;
        CsvImportDialogService = csvImportDialogService;
        EmployeeTabsControlFactory = employeeTabsControlFactory;

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {        
        MainTabControl.Items.Add(GetSettingsTab(SettingsControl, true));
    }    

    private async void OpenCsvMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var csvPath = CsvImportDialogService.ShowCsvImportDialog();
        if (!string.IsNullOrEmpty(csvPath))
        {
            Employees = EmployeeService.ImportEmployeesFromCsv(csvPath, SettingsControl.CurrentSettings!);
            MainTabControl.Items.Clear();

            

            // Neues EmployeeTabsControl für alle Mitarbeiter
            var employeeTabs = await EmployeeTabsControlFactory.CreateAsync(Employees);
            var employeeTabItem = new TabItem 
            { 
                Header = "Mitarbeiterübersicht", 
                Content = employeeTabs,
                IsSelected = true
            };
            MainTabControl.Items.Add(employeeTabItem);

            // Einstellungen-Tab wieder hinzufügen und initialisieren
            MainTabControl.Items.Add(GetSettingsTab(SettingsControl, false));
        }
    }

    private static TabItem GetSettingsTab(SettingsControl settingsControl, bool isSelected)
    {
        return new TabItem
        {
            Header = "Einstellungen",
            Content = settingsControl,
            IsSelected = isSelected,
        };
    }
}
using Microsoft.Win32;
using Net2WorkingHours.Interfaces;
using System.IO;

namespace Net2WorkingHours.Services;

public class CsvImportDialogService : ICsvImportDialogService
{
    public string? ShowCsvImportDialog()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*",
            Title = "CSV-Datei auswählen"
        };
        if (dlg.ShowDialog() == true && File.Exists(dlg.FileName))
        {
            return dlg.FileName;
        }
        return null;
    }
}

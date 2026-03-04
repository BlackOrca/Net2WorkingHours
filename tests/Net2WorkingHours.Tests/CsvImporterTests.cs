using Net2WorkingHours.Core.Utils;

namespace Net2WorkingHours.Tests;

public class CsvImporterTests
{
    [Fact]
    public void ImportCsv_CorrectsUmlautsAndIgnoresUnknown()
    {
        // Arrange
        var csvLines = new List<string>
    {
        "Datum/Uhrzeit;Benutzer;Transpondernummer;Ort des Ereignisses;Ereignis;Details",
        "01.03.2024 08:00;Müller, Jörg;12345;Haupteingang (Eintritt);Zutritt gewährt - Nur Transponder;",
        "01.03.2024 17:00;Unknown;12345;Haupteingang (Austritt);Zutritt gewährt - Nur Transponder;"
    };
        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, csvLines, System.Text.Encoding.GetEncoding("ISO-8859-1"));
        // Act
        var result = CsvImporter.Import(tempFile);
        // Assert
        Assert.Single(result);
        Assert.Equal("Müller, Jörg", result[0].User);
        File.Delete(tempFile);
    }
}
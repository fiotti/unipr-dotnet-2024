
// La Microsoft ha pubblicato una serie di regole che dovrebbero essere
// applicate a tutte le librerie di codice pubblico scritte in C#.
//
// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/standard/design-guidelines/naming-guidelines
// https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names

// Segue la lista delle più importanti tra queste regole.

// Tutti i tipi devono essere definiti in namespace.
// I nomi di namespace devono essere composti da elementi in "PascalCase".
namespace EsempiDotNet.Conventions;

// Tutti i nomi di tipi devono essere in "PascalCase".
public class MyClass
{
    // I nomi delle costanti devono essere in "PascalCase".
    public const int MyConstant = 42;
    
    // I field devono sempre essere privati.
    private int _myValue;

    // Se si vuole esporre un field privato, deve essere fatto tramite property.
    public int MyValue
    {
        get => _myValue;
        set => _myValue = value;
    }

    // Tutti i nomi devono essere chiari piuttosto che brevi.
    public string? Inst { get; set; } // BAD

    public string? InstanceName { get; set; } // GOOD

    public void Up(string path, string url)
    {
        // BAD
    }

    public void UploadFileToServer(string filePath, string serverUrl)
    {
        // GOOD
    }

    // I nomi dei metodi devono essere in "PascalCase".
    // I nomi dei parametri devono essere in "camelCase".
    public int Multiply(int value)
    {
        return value * _myValue;
    }
}

// Tutti i nomi di interfaccia devono cominciare con "I".
public interface IMyInterface
{
    // Il nome dei metodi asincroni deve terminare con "Async".
    Task ProcessAsync(CancellationToken cancellationToken = default);
}

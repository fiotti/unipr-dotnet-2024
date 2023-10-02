
// Oltre al "return" ed ai parametri in "out" o per "ref", in C# esiste un
// quarto modo per restituire un valore da un metodo.
//
// Trattasi delle eccezioni, che vengono generalmente utilizzate per segnalare
// al chiamante che si sono verificate anomalie.

(double X, double Y) NormalizeVector(double x, double y)
{
    double length = Math.Sqrt(x * x + y * y);
    if (length == 0)
        throw new ArgumentException("Vector of length 0 can not be normalized.");

    return (x / length, y / length);
}

NormalizeVector(0.100, 0.000); // (1.000, 0.000)
NormalizeVector(0.200, 0.200); // (0.707, 0.707)
NormalizeVector(0.000, 0.000); // ArgumentException: Vector of length 0 can not be normalized.



// Il chiamante può intercettare le eccezioni con un blocco "try/catch":
try
{
    (double x, double y) = NormalizeVector(0.000, 0.000);
    Console.WriteLine($"Normalized: {x}, {y}");
}
catch (ArgumentException)
{
    Console.WriteLine($"Failed to normalize.");
}

// Output:
// Failed to normalize.



// A differenza dei valori restituiti con "return", le eccezioni si propagano
// ricorsivamente fino al primo blocco "try/catch".
void OuterMethod()
{
    InnerMethod();
}

void InnerMethod()
{
    throw new Exception("This is a test.");
}

try
{
    OuterMethod();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}

// Output:
// Exception: This is a test.



// È possibile filtrare solo alcuni tipi di eccezione indicando nel blocco
// "catch" il tipo di eccezione che si intente intercettare, oppure
// utilizzando la parola chiave "when" per definire una condizione.
try
{
    OuterMethod();
}
catch (Exception ex) when (ex.Message.StartsWith("This"))
{
    Console.WriteLine($"Intercepted!");
}

// Output:
// Intercepted!



// Le eccezioni sono uno strumento di gestione degli errori; usare la parola
// chiave "throw" comporta un elevato costo computazionale a runtime in quanto
// in corrispondenza della "throw" viene letto ed interpretato l'intero stack
// per aggiungere all'eccezione una serie di informazioni utili al debug.
//
// Le eccezioni dovrebbero essere utilizzate solo per situazioni "eccezionali",
// non dovrebbero essere utilizzate per restituire una risposta al chiamante.

// Esempio da evitare:
int ValidateParam(string param)
{
    if (param is not "valid")
        throw new Exception("Validation failed."); // errore

    return 1234; // okay
}

// Per restituire una risposta al chiamante è frequente usare questa pattern:
bool TryValidateParam(string param, out int result)
{
    if (param is not "valid")
    {
        result = default;
        return false; // errore
    }

    result = 1234;
    return true; // okay
}

// È corretto utilizzare le eccezioni per situazioni di anomalia che si
// verificano solo in caso di bug, errori di configurazione, errori hardware,
// ed errori di comunicazione con risorse esterne:
string GetConfig()
{
    if (!File.Exists("config.txt"))
        throw new Exception("Configuration file not found.");
        
    return File.ReadAllText("config.txt");
}



// Non è considerato buona pratica fare catch preventivo di tutte le eccezioni,
// è preferibile gestire selettivamente solo le eccezioni note.

string GetConfigBad()
{
    try
    {
        return File.ReadAllText("config.txt");
    }
    catch (Exception ex) // bad
    {
        throw new Exception("Configuration file not found.", ex);
    }
}

string GetConfigGood()
{
    try
    {
        return File.ReadAllText("config.txt");
    }
    catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException) // good
    {
        throw new Exception("Configuration file not found.", ex);
    }
}



// Negli esempi precedenti si è fatto uso di Exception, ma in genere non è
// considerato buona pratica utilizzare eccezioni generiche, è sempre
// preferibile utilizzare eccezioni specifiche per ciascun problema.
string GetConfig2()
{
    try
    {
        return File.ReadAllText("config.txt");
    }
    catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
    {
        throw new ConfigException("Configuration file not found.", ex);
    }
}

class ConfigException : Exception
{
    public ConfigException()
    {
    }

    public ConfigException(string? message)
        : base(message)
    {
    }

    public ConfigException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}



// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/standard/exceptions/

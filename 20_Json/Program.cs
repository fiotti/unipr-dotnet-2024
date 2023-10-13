using System.Text;
using System.Text.Json;

// Fino a qualche anno fa, per interagire con dati serializzati in JSON in C#
// era comune utilizzare Json.NET (libreria nota anche come "Newtonsoft JSON").
//
// Recentemente la Microsoft ha sviluppato una serie di strumenti inclusi nel
// runtime di .NET per interagire con dati serializzati in JSON.
//
// È ancora frequente trovare su internet esempi che fanno uso della libreria
// Json.NET, ma è consigliabile usare il JSON nativo per tutti i nuovi sviluppi.



// La classe JsonSerializer permette di serializzare classi .NET in JSON.

Example example1 = new()
{
    Hello = "bonjour",
    World = 42,
};

string json1 = JsonSerializer.Serialize(example1);

// Output:
// {"Hello":"bonjour","World":42}
Console.WriteLine(json1);



// La classe JsonSerializer permette anche di deserializzare dal JSON.

string json2 = """{ "Hello": "hola", "World": 123 }""";

Example? example2 = JsonSerializer.Deserialize<Example>(json2);

// Output:
// Hello: hola
// World: 123
Console.WriteLine($"Hello: {example2?.Hello}");
Console.WriteLine($"World: {example2?.World}");

// Il valore restituito è nullabile in quanto il JSON potrebbe essere "null".
Example? example3 = JsonSerializer.Deserialize<Example>("null");

// Output:
// Valore null? True
Console.WriteLine($"Valore null? {example3 == null}");



// Utilizzando il JsonSerializer per serializzare o deserializzare JSON,
// l'intero valore coinvolto deve essere caricato in memoria.
// Capita di dover lavorare su file JSON di grandi dimensioni, per esempio
// è frequente leggere array JSON con decine, centinaia o migliaia di valori.
//
// In questi casi è possibile usare Utf8JsonReader per leggere uno stream
// di byte contenente un JSON in UTF8, senza caricarlo tutto in memoria.

string json4 = """
[
    { "Id": 1, "Name": "Mario", "Surname": "Rossi" },
    { "Id": 2, "Name": "Giuseppe", "Surname": "Verdi" },
    { "Id": 3, "Name": "Antonio", "Surname": "Bianchi" },
    { "Id": 4, "Name": "Maria", "Surname": "Rosati" },
    { "Id": 5, "Name": "Filippo", "Surname": "Vermigli" }
]
""";

// In questo esempio lo stream è caricato interamente in memoria, ma nulla
// vieta di leggere questo JSON per esempio da un file o da un socket TCP.
using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(json4));

// È possibile leggere lo stream JSON un token per volta:
MyJsonStreamTokenEnumerator jsonTokens = new(stream) { DebugPrintToConsole = false };
foreach (Utf8JsonReader token in jsonTokens)
{
    string tokenValue = token.HasValueSequence ? Encoding.UTF8.GetString(token.ValueSequence) : Encoding.UTF8.GetString(token.ValueSpan);
    Console.WriteLine($"Token: '{tokenValue}' ({token.TokenType})");
}

stream.Position = 0;

// È possibile leggere l'array JSON un elemento per volta:
MyJsonArrayStreamEnumerator jsonArrayItems = new(stream);
foreach (JsonElement item in jsonArrayItems)
{
    Person? person = JsonSerializer.Deserialize<Person>(item);
    Console.WriteLine($"{person?.Id}: {person?.Name} {person?.Surname}");
}



class Example
{
    public string? Hello { get; set; }

    public int World { get; set; }
}

class Person
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }
}

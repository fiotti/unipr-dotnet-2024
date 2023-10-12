using System.Text;
using System.Text.Json;

// Fino a qualche anno fa, per interagire con dati serializzati in JSON in C#
// era comune utilizzare Json.NET (libreria nota anche come "Newtonsoft JSON").
//
// Recentemente la Microsoft ha sviluppato una serie di strumenti inclusi nel
// runtime di .NET per interagire con dati serializati in JSON.
//
// È ancora frequente trovare su internet esempi che fanno uso della libreria
// Json.NET, ma è consigiabile usare il JSON nativo per tutti i nuovi sviluppi.



// La classe JsonSerializer permette di serializzre classi .NET in JSON.

Example example1 = new()
{
    Hello = "bonjour",
    World = 42,
};

string json1 = JsonSerializer.Serialize(example1);

// Output:
// {"Hello":"bonjour","World":42}
Console.WriteLine(json1);



// La classe JsonSerializer permette anche di deserializzre dal JSON.

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



class Example
{
    public string? Hello { get; set; }

    public int World { get; set; }
}

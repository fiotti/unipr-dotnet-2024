
// Supponendo di avere necessità di implementare una classe che supporta più
// tipi di dati (in questo esempio int e string)...

TupleInt esempio1 = new(1, 2);
TupleString esempio2 = new("hello", "world");

// ...è possibile usare i generics per scrivere le classi una volta sola.

Tuple<int> esempio3 = new(1, 2);
Tuple<string> esempio4 = new("hello", "world");



public class TupleInt(int item1, int item2)
{
    public int Item1 { get; set; } = item1;
    public int Item2 { get; set; } = item2;
}

public class TupleString(string item1, string item2)
{
    public string Item1 { get; set; } = item1;
    public string Item2 { get; set; } = item2;
}

public class Tuple<T>(T item1, T item2)
{
    public T Item1 { get; set; } = item1;
    public T Item2 { get; set; } = item2;
}



// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/csharp/fundamentals/types/generics

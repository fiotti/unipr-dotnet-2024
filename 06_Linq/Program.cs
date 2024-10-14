
// C# integra un meccanismo di fluent-api progettate per fare operazioni
// di query anche complesse sugli IEnumerable in poche righe di codice.
//
// Questo meccanismo è chiamato Linq.
//
// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/csharp/linq/

// Dato che tutte le "collection" (array, liste, ecc...) sono IEnumerable,
// queste operazioni sono applicabili a tutte le "collection".

// Numeri da 0 a 9 in ordine casuale.
IEnumerable<int> numeri = [6, 8, 5, 1, 2, 7, 9, 4, 0, 3];

// Trova il valore massimo.
// Risultato: 9
int max = numeri.Max();

// Trova il valore minimo.
// Risultato: 0
int min = numeri.Min();

// Calcola il valore medio.
// Risultato: 4.5
double average = numeri.Average();

// Calcola la somma.
// Risultato: 45
int sum = numeri.Sum();

// Ordina in ordine crescente.
// Risultato: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
IEnumerable<int> ordered = numeri.Order();

// Ordina in ordine decrescente.
// Risultato: 9, 8, 7, 6, 5, 4, 3, 2, 1, 0
IEnumerable<int> orderedDescending = numeri.OrderDescending();

// Filtra solo i numeri maggiori di 5.
// Risultato: 6, 8, 7, 9
IEnumerable<int> aboveFive = numeri.Where(n => n > 5);

// Prende il primo valore nella sequenza.
// Risultato: 6
int first = numeri.First();

// Prende il primo valore nella sequenza, se la sequenza è vuota prende il
// valore di default per il tipo di dato della sequenza.
int firstOrDefault = numeri.FirstOrDefault();

// Prende i primi 5 valori nella sequenza.
// Risultato: 6, 8, 5, 1, 2
IEnumerable<int> firstFive = numeri.Take(5);

// Prende i numeri dal 7 in poi nella sequenza.
// Risultato: 7, 9, 4, 0, 3
IEnumerable<int> sevenAndAfter = numeri.SkipWhile(n => n != 7);

// In quanto fluent-api, è possibile concatenare queste operazioni elementari per
// fare query più complesse. L'esempio seguente prende i valori maggiori di 5
// che appaiono dopo il 2 nella sequenza, ordinati in ordine decrescente.
// Risultato: 9, 7
IEnumerable<int> aboveFiveAfterSevenDescending = numeri
    .SkipWhile(n => n != 2)
    .Where(n => n > 5)
    .OrderDescending();



// Quando si utilizza IEnumerable, le query non vengono eseguite immediatamente
// dove è scritto il codice; vengono eseguite solo nel momento in cui si va
// ad iterare una sequenza.

// Esempio:
Console.WriteLine("-- 0 --");

int[] test = [1, 2, 3, 4, 5];

Console.WriteLine("-- 1 --");

IEnumerable<int> test2 = test.Select(n =>
{
    Console.WriteLine($"Selecting {n}");
    return n;
});

Console.WriteLine("-- 2 --");

int[] test3 = test2.Take(2).ToArray();

Console.WriteLine("-- 3 --");

int test4 = test2.First();

Console.WriteLine("-- 4 --");

// Output:
// -- 0 --
// -- 1 --
// -- 2 --
// Selecting 1
// Selecting 2
// -- 3 --
// Selecting 1
// -- 4 --



// Linq permette di scrivere query anche complesse con poche righe di codice.

// Ad esempio, dato un database con tre tabelle contenenti i seguenti dati:
User[] users = [
    new(Id: 1, Username: "Pippo"),
    new(Id: 2, Username: "Pluto"),
    new(Id: 3, Username: "Paperino"),
];

Product[] products = [
    new(Id: 1, Sku: "Mouse"),
    new(Id: 2, Sku: "Keyboard"),
    new(Id: 3, Sku: "Monitor"),
];

Purchase[] purchases = [
    new(Id: 1, UserId: 1, ProductId: 1, PurchaseDate: "2023-09-28"),
    new(Id: 2, UserId: 1, ProductId: 2, PurchaseDate: "2023-09-28"),
    new(Id: 3, UserId: 2, ProductId: 2, PurchaseDate: "2023-09-29"),
    new(Id: 4, UserId: 2, ProductId: 3, PurchaseDate: "2023-09-29"),
    new(Id: 5, UserId: 3, ProductId: 1, PurchaseDate: "2023-09-30"),
];

// Cerca tutti gli acquisti fatti da Pippo:
IEnumerable<Purchase> pippoPurchases2 = purchases
    .Join(users, p => p.UserId, u => u.Id, (p, u) => new { Purchase = p, User = u })
    .Where(x => x.User.Username == "Pippo")
    .Select(x => x.Purchase);

// È anche possibile scrivere la stessa query con una sintassi "SQL" equivalente:
IEnumerable<Purchase> pippoPurchases3 =
    from p in purchases
    join u in users on p.UserId equals u.Id
    where u.Username == "Pippo"
    select p;

record User(int Id, string Username);
record Product(int Id, string Sku);
record Purchase(int Id, int UserId, int ProductId, string PurchaseDate);



// È inoltre presente un'interfaccia IQueryable equivalente ad IEnumerable,
// ma che permette di lavorare su sorgenti remote di dati (database).
//
// Le query con IEnumerable sono codice C# e permettono di lavorare su dati che
// si trovano (anche solo temporaneamente) in memoria.
//
// Le query con IQueryable sono scritte in C#, ma prima di essere eseguite
// potrebbero essere convertite in altri linguaggi, per esempio SQL, ed inviate
// ad una sorgente dati esterna, per esempio un database.
//
// Eccetto questo dettaglio, il codice di una query con IQueryable è quasi
// identico a quello di una query con IEnumerable.

// Per ulteriori informazioni:
// https://learn.microsoft.com/ef/core/get-started/overview/first-app


// Si era in precedenza parlato di Linq, che permette di fare query, anche
// complesse, sugli IEnumerable. Si era menzionato che utilizzando Linq le
// operazioni vengono eseguite on-demand solo sui dati che effettivamente
// vengono letti; non vengono eseguite immediatamente dove viene scritto il
// codice per definire la query.

// Dietro le quinte, IEnumerable può essere implementato come una macchina a
// stati finiti, dove la transizione tra uno stato e l'altro è dettata dalla
// parola chiave "yield".

IEnumerable<int> GetNumbers()
{
    Console.WriteLine("Inside: 1");
    yield return 1;
    Console.WriteLine("Inside: 2");
    yield return 2;
    Console.WriteLine("Inside: 3");
    yield return 3;
    Console.WriteLine("Inside: End");
    yield break;
}

Console.WriteLine("Outside: Start");

foreach (int number in GetNumbers())
{
    Console.WriteLine($"Outside: {number}");
}

Console.WriteLine("Outside: End");

// Output:
// Outside: Start
// Inside: 1
// Outside: 1
// Inside: 2
// Outside: 2
// Inside: 3
// Outside: 3
// Inside: End
// Outside: End

// In corrispondenza della parola chiave "yield", l'esecuzione del metodo
// GetNumbers() viene sospesa, ed il valore viene restituito al chiamante,
// in corrispondenza del ciclo foreach.



// Linq internamente usa del codice simile a quello dell'esempio precedente,
// ma estremamente ottimizzato per avere la migliore performance possibile.

int[] numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

IEnumerable<string> test1 = numbers
    .SkipWhile(n => n != 3)
    .Where(n => n < 6)
    .OrderDescending()
    .Select(n => $"Value A: {n}");

foreach (string s in test1)
{
    Console.WriteLine(s);
}

// Output:
// Value A: 5
// Value A: 4
// Value A: 3

// Segue un'esempio equivalente al codice precedente, implementato usando le
// macchine a stati finiti di C#.

IEnumerable<string> test2 = numbers
    .MySkipWhile(n => n != 3)
    .MyWhere(n => n < 6)
    .MyOrderDescending()
    .MySelect(n => $"Value B: {n}");

foreach (string s in test2)
{
    Console.WriteLine(s);
}

// Output:
// Value B: 5
// Value B: 4
// Value B: 3

static class MyLinq
{
    public static IEnumerable<T> MySkipWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        bool skip = true;

        foreach (T item in source)
        {
            if (skip)
            {
                skip = predicate(item);
            }

            if (!skip)
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (T item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> MyOrderDescending<T>(this IEnumerable<T> source)
    {
        List<T> items = new();

        foreach (T item in source)
        {
            items.Add(item);
        }

        items.Sort();

        for (int i = items.Count - 1; i >= 0; i--)
        {
            yield return items[i];
        }
    }

    public static IEnumerable<U> MySelect<T, U>(this IEnumerable<T> source, Func<T, U> selector)
    {
        foreach (T item in source)
        {
            yield return selector(item);
        }
    }
}

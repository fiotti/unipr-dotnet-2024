
// Dato che in C# la memoria non viene rilasciata esplicitamente tramite "delete",
// è prevista un'interfaccia speciale "IDisposable" per indicare che un oggetto
// deve fare delle operazioni quando non è più utilizzato.

public static class Program
{
    // Se è presente un metodo di nome "Main" viene usato come punto di ingresso.
    public static void Main()
    {
        Example1();
        Example2();
        Example3();
    }

    public static void Example1()
    {
        MyDisposableClass test = new();

        // La classe è disposable, quindi deve essere fatta la dispose quando
        // non si utilizza più la sua istanza.
        test.Dispose();
    }

    public static void Example2()
    {
        // È possibile automatizzare la dispose al termine di un nuovo blocco
        // con la seguente sintassi:
        using (MyDisposableClass test = new())
        {
            // Al termine di questo blocco di codice, viene automaticamente
            // invocato il metodo "Dispose" sulla variabile "test".
        }
    }

    public static void Example3()
    {
        // La seguente sintassi è equivalente all'esempio precedente:
        using MyDisposableClass test = new();

        // Al termine di questo blocco di codice, viene automaticamente
        // invocato il metodo "Dispose" sulla variabile "test".
    }
}

// Questa è un'implementazione minimale di IDisposable solo a scopo dimostrativo:
class MyDisposableClass : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine("Disposing");
    }
}

// Questa è un'implementazione corretta di IDisposable:
class BetterDisposable : IDisposable
{
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // TODO: invocare il metodo "Dispose" degli oggetti che sono
                // stati creati dall'interno di questa classe.
            }

            // TODO: rilasciare le eventuali risorse native, ovvero risorse
            // non gestite da .NET, per esempio quando da un programma scritto
            // in C# si fa uso di componenti scritti in C o C++. Questa è una
            // casistica relativamente rara.

            // TODO: per agevolare il lavoro del garbage collector, è
            // consigliato assegnare valore null a eventuali campi contenenti
            // oggetti di grandi dimensioni, come per esempio buffer.

            _disposed = true;
        }
    }

    // TODO: se sono presenti risorse native (casistica relativamente rara), è
    // necessario aggiungere un finalizzatore alla classe; se non sono presenti
    // risorse native non è necessario aggiungere un finalizzatore.
    // Il finalizzatore deve generalmente contenere solo una chiamata al metodo
    // "Dispose" e nient'altro, per rilasciare le risorse native nel caso in cui
    // lo sviluppatore si sia dimenticato la "using" o di chiamare "Dispose".
    ~BetterDisposable()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

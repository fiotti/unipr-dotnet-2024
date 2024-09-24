
// In C# .NET è possibile avviare nuovi thread, ma generalmente il codice in un
// singolo metodo scritto in C# viene eseguito su un singolo thread.

// Con C# 5 sono state introdotte le nuove parole chiave async ed await, introducendo
// il vasto universo dei task in contrapposizione al concetto di thread.
//
// Un thread è un filo logico eseguito su un processore che può essere eseguito
// su uno o più core logici del processore. È un concetto fortemente legato
// all'hardware ed al kernel del sistema operativo.
//
// Un task è semplicemente un'operazione, che in base alla configurazione
// potrebbe essere vincolata ad essere eseguita su un determinato thread,
// oppure liberamente sul primo thread libero in una thread-pool.
//
// Come un thread può essere interrotto dal kernel e ripreso su un altro core
// logico della CPU, un task può essere interrotto e ripreso su un altro thread.
// Questi cambi possono avvenire solo in corrispondenza della parola chiave await.

// Il vantaggio di usare task asincroni è che il runtime di .NET è libero di
// riciclare thread, piuttosto che doverne creare uno nuovo per ogni operazione.
// Creare un nuovo thread è un'operazione potenzialmente molto pesante, che
// prevede una chiamata al kernel del sistema operativo ed una modifica alla
// coda di esecuzione globale.

// È bene usare async in qualsiasi metodo che esegue operazioni di input/output.
// Non è utile convertire in async un metodo che fa solo uso di CPU e memoria.

// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/async-scenarios



// Un metodo è considerato "asincrono" se usa la parola chiave async e/o se
// restituisce un Task oppure un ValueTask.

void Example1()
{
    // Questo non è un metodo asincrono.
}

async void Example1Async()
{
    // Questo è un metodo asincrono.

    // Nota: non è MAI considerato buona pratica fare un metodo asincrono con
    // tipo di ritorno void. Questo metodo è solo di esempio.
}

double Example2()
{
    return 12.34;
}

async Task<double> Example2Async()
{
    return 12.34;
}

int Example3()
{
    return 123;
}

Task<int> Example3Async()
{
    return Task.FromResult(123);
}

string Example4()
{
    return "hello";
}

ValueTask<string> Example4Async()
{
    return ValueTask.FromResult("hello");
}



// I metodi asincroni possono essere chiamati all'interno di un contesto "async"
// tramite la parola chiave "await".
string DoSomething()
{
    double test1 = Example2();
    int test2 = Example3();
    string test3 = Example4();

    return $"{test1} {test2} {test3}";
}

async Task<string> DoSomethingAsync()
{
    double test1 = await Example2Async();
    int test2 = await Example3Async();
    string test3 = await Example4Async();

    return $"{test1} {test2} {test3}";
}



// Il vantaggio di usare metodi asincroni è quando sono presenti operazioni che
// non restituiscono immediatamente un valore, per esempio letture da file,
// socket e networking, interazioni con periferiche di sistema, ecc...
string GetPage(string url)
{
    using HttpClient client = new();

    // La riga seguente fa una chiamata HTTP; mentre attende risposta il thread
    // chiamante resta bloccato, occupando inutilmente risorse del sistema.
    HttpResponseMessage response = client.Send(new(HttpMethod.Get, url));

    // Le righe seguenti leggono la risposta in modo sincrono.
    using Stream stream = response.Content.ReadAsStream();
    using StreamReader reader = new(stream, leaveOpen: true);
    string content = reader.ReadToEnd();

    return content;
}

// Il cancellation token permette di annullare un'operazione asincrona
// dall'esterno se il chiamante non è più interessato ad una risposta.
// È buona pratica aggiungerne uno ai metodi asincroni.
async Task<string> GetPageAsync(string url, CancellationToken cancellationToken = default)
{
    // Nota: la versione asincrona del codice seguente potrebbe essere scritta
    // in modo più semplice, ma ho cercato di tenerla il più simile possibile
    // alla versione sincrona, per permettere un confronto riga per riga.

    using HttpClient client = new();

    // La riga seguente fa una chiamata HTTP; mentre attende risposta viene
    // rilasciato il thread chiamante, che nel frattempo può fare altro.
    HttpResponseMessage response = await client.SendAsync(new(HttpMethod.Get, url), cancellationToken);

    // Le righe seguenti leggono la risposta in modo asincrono.
    await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    using StreamReader reader = new(stream, leaveOpen: true);
    string content = await reader.ReadToEndAsync(cancellationToken);

    return content;
}



// In un contesto sincrono potrebbe risultare difficile eseguire più operazioni
// in parallelo, ma in un contesto asincrono è semplice.
async Task<string> GetPagesSequentialAsync(CancellationToken cancellationToken = default)
{
    // Fa la prima chiamata HTTP ed attende una risposta.
    string home = await GetPageAsync("https://example.com", cancellationToken);

    // Fa la seconda chiamata HTTP ed attende una risposta.
    string test = await GetPageAsync("https://example.com/test", cancellationToken);

    return $"{home}\n\n{test}";
}

async Task<string> GetPagesParallelAsync(CancellationToken cancellationToken = default)
{
    // Fa due chiamate HTTP in parallelo.
    Task<string> homeTask = GetPageAsync("https://example.com", cancellationToken);
    Task<string> testTask = GetPageAsync("https://example.com/test", cancellationToken);

    // Attende che entrambe le chiamate abbiano ricevuto una risposta.
    await Task.WhenAll(homeTask, testTask);

    // Legge la risposta ad entrambe le chiamate.
    string home = await homeTask;
    string test = await testTask;

    return $"{home}\n\n{test}";
}



// Supponendo di avere un metodo legacy scritto con codice sincrono che non può
// essere convertito in codice asincrono...
string SomeLegacyMethod()
{
    // codice...
    return "test";
}

// ...è possibile eseguire codice sincrono in un contesto asincrono:
async Task<string> SomeLegacyMethodAsync(CancellationToken cancellationToken = default)
{
    // Nota: non è MAI necessario e non è considerato buona pratica aggiungere
    // un metodo asincrono che fa il wrap della versione sincrona.
    //
    // La buona pratica vuole che il codice legacy venga implementato in modo
    // asincrono, ed eventualmente che la versione sincrona chiami quella
    // asincrona.
    //
    // Ove ciò non fosse possibile, è consigliato lasciare che sia il chiamante
    // a passare il metodo sincrono a Task.Run() solo se lo ritiene necessario.

    // Nota 2: il cancellation token non è in grado di cancellare codice
    // sincrono, il massimo che può fare è non lanciare l'operazione se rileva
    // che la richiesta è già stata cancellata quando questo codice parte.

    Task<string> task = Task.Run(SomeLegacyMethod, cancellationToken);

    return await task;
}



// È anche possibile eseguire codice asincrono in un contesto sincrono:
string GetPagesSequential()
{
    // Avvia la chiamata al metodo asincrono.
    Task<string> task = GetPagesParallelAsync();

    // Attende in modo sincrono risposta (blocca il thread chiamante).
    return task.GetAwaiter().GetResult();
}



// Microsoft consiglia di usare async/await in ASP.NET Core, di non usare
// Task.Run(), Task.Wait() o Task.Result, e di non usare lock in codice
// condiviso.
// https://learn.microsoft.com/aspnet/core/fundamentals/best-practices#avoid-blocking-calls



// È possibile usare il cancellation token per cancellare un'operazione
// asincrona se il chiamante non è più interessato:
async Task ProcessNumbersAsync(int[] numbers, CancellationToken cancellationToken = default)
{
    foreach (int number in numbers)
    {
        // Lancia "OperationCancelledException" se richiesta la cancellazione.
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($"Processing {number}...");

        // Simula operazione di lunga durata...
        await Task.Delay(1000, cancellationToken);
    }
}

int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Sorgente di cancellation token, che viene cancellato dopo 3 secondi.
using CancellationTokenSource cts = new();
cts.CancelAfter(TimeSpan.FromSeconds(3));

try
{
    await ProcessNumbersAsync(numbers, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation cancelled.");
}

// Nota: il cancellation token indica una "richiesta da parte del chiamante,
// di cancellare l'operazione in corso". Trattasi solo di una richiesta, il
// metodo chiamato non garantisce che la richiesta venga soddisfatta
// immediatamente, anzi, non garantisce affatto alcuna cancellazione.
//
// È buona pratica accettare un cancellation token in tutti i metodi async, e
// tenerne conto internamente per interrompere l'operazione in corso, ma il
// chiamante non può dare per scontato che il chiamato adotti questa pratica.

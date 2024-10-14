﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

// Si supponga di voler implementare una classe DataUploader che legge i dati
// da una sorgente generica IDataAccessor e li invia ad un server generico
// tramite IServerUploader.

IDataAccessor dataAccessor = new FileDataAccessor();
IServerUploader serverUploader = new HttpServerUploader();
IDataUploader dataUploader = new DataUploader(dataAccessor, serverUploader);

await dataUploader.UploadDataToServerAsync();

// Utilizzando questo modello è possibile separare le responsabilità rendendo
// più facile ragionare sul codice.

// Inoltre, se in futuro dovesse sorgere la necessità di modificare il
// comportamento dell'applicazione, potrebbe essere sufficiente sostituire
// il singolo componente responsabile per l'operazione da modificare.
//
// Per esempio, il FileDataAccessor potrebbe un domani essere rimpiazzato con
// un DatabaseDataAccessor, senza bisogno di modificare il codice delle classi
// che ne fanno uso.



// Microsoft mette a disposizione un set di funzionalità per rendere il più
// trasparente possibile la gestione di queste dipendenze, tramite la pattern
// di design del software chiamata "dependency injection".

// Il codice seguente è funzionalmente equivalente al precedente.

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// I servizi dell'applicazione si configurano così:
builder.Services.TryAddSingleton<IDataAccessor, FileDataAccessor>();
builder.Services.TryAddSingleton<IServerUploader, HttpServerUploader>();
builder.Services.TryAddSingleton<IDataUploader, DataUploader>();

// Nelle applicazioni tradizionali il codice si trova nel main, mentre nelle
// "host application" il codice risiede negli "hosted service".
// È anche consentito registrare più di un "hosted service".
builder.Services.AddHostedService<MainService>();

// Una volta configurati tutti i servizi, la "hosted application" può essere
// compilata. Una volta compilata non è più possibile modificare i servizi.
using IHost host = builder.Build();

// A questo punto è possibile eseguire tutti gli "hosted service" aggiunti
// all'applicazione.
await host.RunAsync();


// Nota: le dipendenze dei servizi vengono automaticamente risolte
// ed iniettate dal "service provider" di Microsoft; è sufficiente
// indicarle come parametri al costruttore.
class MainService(IDataUploader dataUploader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await dataUploader.UploadDataToServerAsync(stoppingToken);

        // Nelle applicazioni tradizionali, all'uscita dal main l'esecuzione
        // dell'applicazione termina automaticamente.
        // Nelle "hosted application" l'esecuzione termina solo su richiesta.

        // Per le applicazioni da linea di comando è possibile inviare una
        // richiesta di terminazione con la combinazione di tasti CTRL + C.
    }
}



// I servizi dell'applicazione possono essere registrati con "lifetime":
//
// Singleton - servizi per i quali esiste una ed una sola istanza in tutta
// l'applicazione; questa istanza è condivisa tra tutti gli utilizzatori
// e deve generalmente essere thread-safe in quanto potrebbe essere utilizzata
// contemporaneamente da più punti nel codice.
//
// Transient - servizi con istanze effimere; per ogni utilizzatore che fa uso
// di questo servizio, viene automaticamente creata una nuova istanza, che
// verrà poi automaticamente "disposed" insieme all'utilizzatore.
//
// Scoped - servizi per i quali viene creata una singola istanza per "scope";
// in ASP.NET Core lo "scope" è una singola richiesta HTTP, ovvero se i
// controller che gestiscono una richiesta fanno uso di un servizio
// registrato con lifetime "scoped", verrà creata una nuova istanza per
// richiesta HTTP, e verrà poi "disposed" dopo aver inviato una risposta.

// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/core/extensions/dependency-injection

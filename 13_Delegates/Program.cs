
// In C# esistono vari modi per definire funzioni o metodi, la sintassi più
// classica quella estesa:

void StampaHelloWorld()
{
    Console.WriteLine("Hello, World!");
}

// Le funzioni o metodi possono essere invocati con la seguente sintassi:
StampaHelloWorld();



// È possibile assegnare ad una variabile di tipo Action una funzione void,
// ovvero che non restituisce valori, e senza parametri:
Action action = StampaHelloWorld;

// Le variabili di tipo Action sono invocabili con la stessa sintassi:
action();



// È anche possibile definire una funzione in linea nel codice, facendo uso
// della sintassi lambda:
Action daLambda = () => Console.WriteLine("Lambda!");

daLambda();



// È anche possibile dichiarare funzioni con parametri:
Action<int> conParam = (int parametro) => Console.WriteLine($"Parametro: {parametro}");

conParam(1);
conParam(2);
conParam(3);



// Il tipo Func permette di utilizzare funzioni che restituiscono un valore:
Func<int, string> raddoppia = (int valore) => (valore * 2).ToString();

Console.WriteLine($"A: {raddoppia(10)}");
Console.WriteLine($"B: {raddoppia(20)}");
Console.WriteLine($"C: {raddoppia(30)}");



// È possibile utilizzare le funzioni come qualsiasi altro tipo,
// nell'esempio seguente vengono memorizzate in un dizionario:
Dictionary<string, Func<double, double, double>> operazioni = new()
{
    ["+"] = (double a, double b) => a + b,
    ["-"] = (double a, double b) => a - b,
    ["*"] = (double a, double b) => a * b,
    ["/"] = (double a, double b) => a / b,
};

void StampaEspressione(double a, string operatore, double b)
{
    Func<double, double, double> func = operazioni[operatore];
    Console.WriteLine($"{a} {operatore} {b} = {func(a, b)}");
}

StampaEspressione(1, "+", 2);
StampaEspressione(5, "-", 3);
StampaEspressione(10, "*", 5);
StampaEspressione(20, "/", 4);



// È possibile anche restituire più di un valore:
(int First, int Second) GetManyValues()
{
    return (123, 456);
}

(int first, int second) = GetManyValues();

// Se il chiamante non è interessato a leggere tutti i valori restituiti,
// può ignorarli usando la seguente sintassi:
(int v1, _) = GetManyValues();
(_, int v2) = GetManyValues();



// I parametri passati alle funzioni sono accessibili dall'interno,
// ma eventuali modifiche non vengono propagate all'esterno:
void Metodo1(string param)
{
    Console.WriteLine($"A: {param}");

    param = "INTERNAL";
    Console.WriteLine($"B: {param}");
}

string c = "EXTERNAL";
Metodo1(c);

Console.WriteLine($"C: {c}");


// Output:
// A: EXTERNAL
// B: INTERNAL
// C: EXTERNAL



// I parametri in "out" sono modificabili dall'interno di un metodo,
// non possono essere letti dall'interno del metodo:
string Metodo2(out string outParam)
{
    outParam = "OUT";
    return "RETURN";
}

string o;
string r = Metodo2(out o);
Console.WriteLine($"R: {r}");
Console.WriteLine($"O: {o}");

// Output:
// R: RETURN
// O: OUT



// I parametri per "ref" sono sia leggibili che modificabili dall'interno:
void Metodo3(ref string refParam)
{
    Console.WriteLine($"P: {refParam}");
    refParam = "REF_2";
}

string v = "REF_1";
Metodo3(ref v);
Console.WriteLine($"V: {v}");

// Output:
// P: REF_1
// V: REF_2



// Quando si vuole memorizzare in una variabile una funzione che fa uso di
// parametri in "out" o per "ref" non è possibile usare Action o Func, è
// necessario costruire un delegate appropriato:
MyDelegate makeDouble = (int param, out int doubleParam) => doubleParam = param * 2;

int double123;
makeDouble(123, out double123);

public delegate void MyDelegate(int param, out int doubleParam);



// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/csharp/programming-guide/delegates/

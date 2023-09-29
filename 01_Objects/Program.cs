
// In C# è possibile definire classi con questa sintassi:
class MyClass
{
    // Questo è un campo.
    private int _field;

    // I campi "readonly" possono essere assegnati solo dal costruttore.
    private readonly int _readonlyField;

    // Questa è una property con getter e setter generati automaticamente.
    public int AutoProperty { get; set; }

    // Questa è una property con getter e setter indicati manualmente.
    public int Property
    {
        get
        {
            return _field;
        }
        set
        {
            _field = value;
        }
    }

    // È supportata anche una sintassi più semplice.
    public int Property2
    {
        get => _field;
        set => _field = value;
    }

    // Le property possono anche avere solo un getter, o volendo solo un setter (raramente).
    public int GetterOnly
    {
        get => _readonlyField;
    }

    // Per le property con solo un getter è supportata anche una sintassi più semplice.
    public int GetterOnly2 => _readonlyField;

    // Questo è il costruttore della classe.
    public MyClass(int value)
    {
        _readonlyField = value;
    }

    // Questo è un metodo.
    public void PrintValue()
    {
        Console.WriteLine($"Il valore della property è: {Property}");
    }

    // È possibile definire un metodo "static" accessibile ovunque senza bisogno di creare istanze della classe.
    public static void Hello()
    {
        Console.WriteLine("Hello!");
    }
}



// Ereditarietà e polimorfismo sono supportati.

// Una classe "abstract" non permette la creazione di istanze.
public abstract class Animal
{
    // Un metodo o una property può essere "abstract" per indicare che non è
    // definito un effetto. Questo è possibile solo in classi astratte.
    public abstract string Family { get; }

    public abstract string Name { get; }

    public abstract void Talk();
}

public class Dog : Animal
{
    // "override" indica che si sta definendo l'effetto di un metodo o
    // di una property definita in una classe astratta.
    public override string Family => "Canidae";

    // Una auto-property con solo un getter si comporta come un campo readonly,
    // ovvero è assegnabile solamente dal costruttore della classe.
    public override string Name { get; }

    public Dog(string name)
    {
        Name = name;
    }

    public override void Talk()
    {
        Console.WriteLine("Bau bau!");
    }
}

public class Cat : Animal
{
    public override string Family => "Felidae";

    public override string Name { get; }

    public Cat(string name)
    {
        Name = name;
    }

    public override void Talk()
    {
        Console.WriteLine("Miao!");
    }
}



// Non è supportata l'ereditarietà multipla, ma per ovviare questo problema è possibile fare uso di interfacce.
// Una classe può ereditare da una sola classe padre, ma può implementare più di un'interfaccia.
public interface IAnimal
{
    string Family { get; }

    string Name { get; }

    void Talk();
}

public interface IJumping
{
    void Jump();
}

public class Human : Animal, IAnimal, IJumping
{
    public override string Family => "Hominidae";

    public override string Name { get; }

    public Human(string name)
    {
        Name = name;
    }

    public override void Talk()
    {
        Console.WriteLine("Ciao!");
    }

    public void Jump()
    {
        Console.WriteLine("Hop!");
    }
}




// Le "struct" sono equivalenti alle "class", ma non supportano ereditarietà; possono implementare interfacce.
// A differenza delle classi, le strutture risiedono sullo stack piuttosto che sulla heap.

public struct MyStruct : IJumping
{
    public void Jump()
    {
        Console.WriteLine("Jump called.");
    }
}



// I "delegate" permettono di definire la firma di una funzione.
public delegate int MyDelegate(int param, string anotherParam);



// "enum" è equivalente ad una "struct" contenente solo costanti tutti di tipo intero.
public enum MyEnum
{
    Default, // implicitamente 0
    First = 10,
    Second = 20,
    Third, // implicitamente 21
    Fourth, // implicitamente 22
    Hundredth = 100,
}

// Equivalente approssimativamente a:
public struct MyEnum2
{
    public const int Default = 0;
    public const int First = 10;
    public const int Second = 20;
    public const int Third = 21;
    public const int Fourth = 22;
    public const int Hundredth = 100;
}

// "record" è equivalente ad una "class" di sole property assegnate al costruttore.
public record MyRecord(int Test, string Val);

// Equivalente approssimativamente a:
public class MyRecord2
{
    public int Test { get; }

    public string Val { get; }

    public MyRecord2(int test, string val)
    {
        Test = test;
        Val = val;
    }
}



// In C# sono previsti i seguenti modificatori di visibilità:
// public       visibile ovunque
// protected    visibile all’interno della stessa classe e sottoclassi
// private      visibile solo all’interno della stessa classe
// internal     visibile ovunque nello stesso assembly

// Inoltre è possibile combinare "protected" ed "internal":
// protected internal   visibile ovunque nello stesso assembly, e visibile nelle sottoclassi anche se sono definite in altri assembly

// È possibile omettere i modificatori di visibilità, si applicano le seguenti regole:
// Classi, interfacce, strutture, delegati, enum e record sono implicitamente "internal".
// Metodi e property di un'interfaccia o enum sono implicitamente "public".
// Campi, metodi e property di una classe o struttura sono implicitamente sono implicitamente "private".

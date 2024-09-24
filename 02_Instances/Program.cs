
public static class Program
{
    // Se è presente un metodo di nome "Main" viene usato come punto di ingresso.
    public static void Main()
    {
        SomeMethod();
        SomeMethod2();
        SomeMethod3();
    }

    private static void SomeMethod()
    {
        // Alloca "myObject" sulla heap dato che il tipo della variabile è una classe.
        MyClass myObject = new(123);

        Console.WriteLine($"First: {myObject.Num}"); // 123

        // È possibile passare gli oggetti ai metodi, vengono passati per riferimento.
        HelloClass(myObject);

        Console.WriteLine($"Second: {myObject.Num}"); // 456

        // "myObject" verrà deallocato automaticamente a breve, in quanto non più utilizzato.
        // Non è necessario fare esplicitamente "delete" in C# per deallocare la memoria.
    }

    private static void SomeMethod2()
    {
        // Alloca "myValue" sullo stack dato che il tipo della variabile è una struct.
        MyStruct myValue = new(123);

        Console.WriteLine($"Third: {myValue.Num}"); // 123

        // È possibile passare i valori ai metodi, ma ad ogni chiamata viene fatta una copia.
        HelloStruct(myValue);

        Console.WriteLine($"Fourth: {myValue.Num}"); // 123 (e non 456)

        // Dato che "myValue" è sullo stack, non è necessario deallocare nulla.
        // Questa area di memoria non è più raggiungibile una volta usciti dal metodo.
    }

    private static void SomeMethod3()
    {
        // Alloca "myValue" sulla heap dato che il tipo della variabile è un'interfaccia.
        IMyInterface myValue = new MyStruct(123);

        Console.WriteLine($"Firth: {myValue.Num}"); // 123

        HelloInterface(myValue);

        Console.WriteLine($"Sixth: {myValue.Num}"); // 456
    }

    private static void HelloClass(MyClass obj)
    {
        obj.Num = 456;
    }

    private static void HelloStruct(MyStruct val)
    {
        val.Num = 456;
    }

    private static void HelloInterface(IMyInterface val)
    {
        val.Num = 456;
    }
}

public interface IMyInterface
{
    int Num { get; set; }
}

public class MyClass(int initialNum)
{
    public int Num { get; set; } = initialNum;
}

public struct MyStruct(int initialNum) : IMyInterface
{
    public int Num { get; set; } = initialNum;
}

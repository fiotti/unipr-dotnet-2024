
public static class Program
{
    // Se è presente un metodo di nome "Main" viene usato come punto di ingresso.
    public static void Main()
    {
        SomeMethod();
        SomeMethod2();
    }

    private static void SomeMethod()
    {
        // Alloca "myObject" sulla heap.
        MyClass myObject = new(123);

        Console.WriteLine($"First: {myObject.Num}"); // 123

        // È possibile passare gli oggetti ai metodi, vengono passati per riferimento.
        Hello(myObject);

        Console.WriteLine($"Second: {myObject.Num}"); // 456

        // "myObject" verrà deallocato automaticamente a breve, in quanto non più utilizzato.
        // Non è necessario fare esplicitamente "delete" in C# per deallocare la memoria.
    }

    private static void SomeMethod2()
    {
        // Alloca "myValue" sullo stack.
        MyStruct myValue = new(123);

        Console.WriteLine($"Third: {myValue.Num}"); // 123

        // È possibile passare i valori ai metodi, ma ad ogni chiamata viene fatta una copia.
        Hello2(myValue);

        Console.WriteLine($"Fourth: {myValue.Num}"); // 123 (e non 456)

        // Dato che "myValue" è sullo stack, non è necessario deallocare nulla.
        // Questa area di memoria non è più raggiungibile una volta usciti dal metodo.
    }

    private static void Hello(MyClass obj)
    {
        obj.Num = 456;
    }

    private static void Hello2(MyStruct val)
    {
        val.Num = 456;
    }
}

public class MyClass
{
    public int Num { get; set; }

    public MyClass(int initialNum)
    {
        Num = initialNum;
    }
}

public struct MyStruct
{
    public int Num { get; set; }

    public MyStruct(int initialNum)
    {
        Num = initialNum;
    }
}

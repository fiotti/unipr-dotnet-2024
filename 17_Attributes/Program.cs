using System.Reflection;
using System.Text;

// Gli attributi permettono di aggiungere dei metadati al codice.
//
// Per ulteriori informazioni:
// https://learn.microsoft.com/dotnet/csharp/advanced-topics/reflection-and-attributes/

MyClass myObject = new("hi", 123);
string serialized = MySerializer.Serialize(myObject);
Console.WriteLine($"Serialized: {serialized}");

// Output:
// Serialized: [World: 123, Hello: hi]



[SerializableWithMySerializer]
public class MyClass(string hello, int worldValue)
{
    [MySerializerName("Hello")]
    private string _hello = hello;

    [MySerializerName("World")]
    public int WorldValue => worldValue;
}



[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
class SerializableWithMySerializerAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
class MySerializerNameAttribute(string name) : Attribute
{
    public string Name => name;
}



static class MySerializer
{
    public static string Serialize<T>(T obj)
    {
        bool isSerializable = typeof(T).GetCustomAttribute<SerializableWithMySerializerAttribute>() != null;
        if (!isSerializable)
            throw new ArgumentException($"{typeof(T).Name} not serializable with my serializer.");

        MemberInfo[] members = typeof(T)
            .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        
        StringBuilder sb = new();

        sb.Append('[');

        bool isFirst = true;
        foreach (MemberInfo member in members)
        {
            MySerializerNameAttribute? attribute = member.GetCustomAttribute<MySerializerNameAttribute>();
            if (attribute == null)
                continue;

            if (isFirst)
                isFirst = false;
            else
                sb.Append(", ");

            sb.Append(attribute.Name);
            sb.Append(": ");

            object? memberValue = member switch
            {
                PropertyInfo p => p.GetValue(obj),
                FieldInfo f => f.GetValue(obj),
                _ => throw new NotSupportedException()
            };

            sb.Append(memberValue);
        }

        sb.Append(']');

        return sb.ToString();
    }
}

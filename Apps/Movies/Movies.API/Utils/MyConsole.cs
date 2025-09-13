using System.Diagnostics;

namespace Movies.API.Utils;

public class MyConsole
{
    public static void WriteLine(string? value)
    {
        Debug.WriteLine(value);
        Console.WriteLine(value);
    }


}

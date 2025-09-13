// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, Docker!");
Console.WriteLine("What's your name dog?");

var name = Console.ReadLine();

name = string.IsNullOrWhiteSpace(name) ? "Stranger" : name;

Console.WriteLine($"How'ya {name}");

Console.WriteLine("Press any key to exit...");
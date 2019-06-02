using System;
using NSMainLoop;

namespace ExampleCore
{
    class Program
    {

        static void Main(string[] args)
        {
            MainLoop.Instance.Start(Run);
        }

        static async void Run()
        {
            System.Console.WriteLine($"{DateTime.Now}");
            await new MDelay(1000);
            System.Console.WriteLine("delay hello world");
            System.Console.WriteLine($"{DateTime.Now}");
            await new MDelay(2000);
            System.Console.WriteLine("delay hello world");
            System.Console.WriteLine($"{DateTime.Now}");
            await new MDelay(4000);
            System.Console.WriteLine("delay hello world");
            System.Console.WriteLine($"{DateTime.Now}");
            await new MDelay(8000);
            System.Console.WriteLine("delay hello world");
            System.Console.WriteLine($"{DateTime.Now}");
        }
    }
}

using System;

namespace S3Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.Write("Comando>");
            String command;
            Boolean quitNow = false;
            while (!quitNow)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    case "Test":
                        Console.WriteLine("Starting Test.");
                        
                        Console.WriteLine("Comando>");
                        break;

                    case "quit":
                        quitNow = true;
                        break;

                    default:
                        Console.WriteLine("Unknown Command " + command);
                        break;
                }
            }


        }


    }
}

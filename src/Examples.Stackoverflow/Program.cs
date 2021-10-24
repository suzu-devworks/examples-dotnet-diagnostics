using System;

namespace Examples.Stackoverflow
{
    class Program
    {
        static void CallInfinitely()
        {
            CallInfinitely();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CallInfinitely();

            return;
        }
    }
}

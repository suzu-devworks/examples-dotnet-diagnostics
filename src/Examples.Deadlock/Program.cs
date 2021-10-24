using System;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Deadlock
{
    class Program
    {
        private static object A = new();
        private static object B = new();

        private static void MethodA()
        {
            Console.WriteLine("Inside Method A");
            lock (A)
            {
                Console.WriteLine("Method A: Inside LockA and Trying to enter LockB");
                Thread.Sleep(5000);
                lock (B)
                {
                    Console.WriteLine("Method A: Inside LockA and Inside LockB");
                    Thread.Sleep(5000);
                }
                Console.WriteLine("Method A: Inside LockA and Outside LockB");
            }
            Console.WriteLine("Method A: Outside LockA and Outside LockB");
        }

        private static void MethodB()
        {
            Console.WriteLine("Inside Method B");
            lock (B)
            {
                Console.WriteLine("Method B: Inside LockB and Trying to enter LockA");
                Thread.Sleep(5000);
                lock (A)
                {
                    Console.WriteLine("Method B: Inside LockB and Inside LockA");
                    Thread.Sleep(5000);
                }
                Console.WriteLine("Method B: Inside LockB and Outside LockA");
            }
            Console.WriteLine("Method B: Outside LockB and Outside LockA");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task.Run(() => MethodA());
            Task.Run(() => MethodB());

            Thread.Sleep(1000);
            Console.WriteLine("wait enter...");
            Console.ReadLine();

            return;
        }
    }
}

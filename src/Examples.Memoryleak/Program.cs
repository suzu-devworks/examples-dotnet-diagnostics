using System;
using System.Collections.Generic;

#pragma warning disable IDE0044
#pragma warning disable IDE0052

namespace Examples.Memoryleak
{
    class Program
    {
        private class LeakObject
        {
            string _name;
            int _number;
            byte[] b = new byte[1000];
            static int count = 0;

            public LeakObject()
            {
                _name = count.ToString();
                _number = count;
                count++;
            }
        }

        private static List<LeakObject> _objects = new();


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            for (var i = 0; i < 1000000; i++)
            {
                _objects.Add(new LeakObject());
            }

            Console.WriteLine("wait enter...");
            Console.ReadLine();

            return;
        }
    }
}

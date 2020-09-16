using System;
using Consul;

namespace ConsulApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var consul = new ConsulClient();
        }


    }
}

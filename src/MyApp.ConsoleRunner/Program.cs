// See https://aka.ms/new-console-template for more information
//
using System;
using MyApp.Core;

namespace MyApp.ConsoleRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, reader!");

            var calculator = new Calculator();

            Console.WriteLine($"1 + 1 is {calculator.Add(1, 1)}");
            Console.WriteLine($"4 - 1 is {calculator.Subtract(4, 1)}");
            Console.WriteLine($"7 * 2 is {calculator.Multiply(7, 2)}");
        }
    }
}

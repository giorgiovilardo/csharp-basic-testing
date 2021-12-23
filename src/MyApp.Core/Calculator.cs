namespace MyApp.Core;

public class Calculator : ICalculator
{
    public int Add(int x, int y) => x + y;
    public int Subtract(int x, int y) => x - y;
    public int Multiply(int x, int y) => x * y;
}

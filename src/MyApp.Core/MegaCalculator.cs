namespace MyApp.Core;

public class MegaCalculator
{
    private readonly ICalculator _calculator;

    public MegaCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }
    
    public int MegaAlgorithm(int firstNumber, int secondNumber)
    {
        var added = _calculator.Add(firstNumber, secondNumber);
        var subOne = _calculator.Subtract(firstNumber, 1);
        var multBoth = _calculator.Multiply(added, subOne);

        return multBoth;
    }
}

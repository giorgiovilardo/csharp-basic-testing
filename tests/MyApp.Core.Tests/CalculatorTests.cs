using System.Collections.Generic;
using Xunit;

namespace MyApp.Core.Tests;

public class CalculatorTests
{
    [Fact]
    public void AddOneAndOneIsTwo()
    {
        var calculator = new Calculator();
        var result = calculator.Add(1, 1);
        Assert.Equal(2, result);
    }
    
    [Theory]
    [InlineData(4, 2, 2)]
    [InlineData(7, 2, 5)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, -1, 0)]
    [InlineData(0, 0, 0)]
    public void SubtractsCorrectly(int firstNumber, int secondNumber, int expectedResult)
    {
        var calculator = new Calculator();
        var actualResult = calculator.Subtract(firstNumber, secondNumber);
        Assert.Equal(expectedResult, actualResult);
    }
    
    [Theory]
    [MemberData(nameof(MultiplyTestCases))]
    private void MultipliesCorrectly(int firstNumber, int secondNumber, int expectedResult)
    {
        var calculator = new Calculator();
        var actualResult = calculator.Multiply(firstNumber, secondNumber);
        Assert.Equal(expectedResult, actualResult);
    }
    
    public static IEnumerable<object[]> MultiplyTestCases =>
        new List<object[]>
        {
            new object[] { 1, 2, 2 },
            new object[] { 2, 2, 4 },
            new object[] { 8, 2, 16 },
            new object[] { 10, 10, 100 },
            new object[] { 15, 10, 150 },
        };
}

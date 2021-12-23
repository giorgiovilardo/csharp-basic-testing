using Moq;
using Xunit;

namespace MyApp.Core.Tests;

public class MegaCalculatorTests
{
    [Theory]
    [InlineData(4, 10, 42)]
    public void TestMegaAlgorithm(int firstNumber, int secondNumber, int expectedResult)
    {
        // Arrange
        var mockCalculator = new Mock<ICalculator>();
        mockCalculator.Setup(c => c.Add(firstNumber, secondNumber)).Returns(14);
        mockCalculator.Setup(c => c.Subtract(firstNumber, 1)).Returns(3);
        mockCalculator.Setup(c => c.Multiply(14, 3)).Returns(expectedResult);
        var megaCalculator = new MegaCalculator(mockCalculator.Object);
        
        // Act
        var actualResult = megaCalculator.MegaAlgorithm(firstNumber, secondNumber);
        
        // Assert
        mockCalculator.Verify(c => c.Add(firstNumber, secondNumber));
        mockCalculator.Verify(c => c.Subtract(firstNumber, 1));
        mockCalculator.Verify(c => c.Multiply(14, 3));
        Assert.Equal(expectedResult, actualResult);
    }
}

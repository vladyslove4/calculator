namespace Calculator.Tests.Entities;

public class CalculatorTests
{
    Calculator.Entities.Calculator calculator;
    public CalculatorTests()
    {
        calculator = new Calculator.Entities.Calculator();
    }

    [Theory]
    [InlineData("2+15/3+4*2", 15)]
    [InlineData("2+15/0", double.PositiveInfinity)]
    [InlineData("2321dsa//", double.NaN)]
    [InlineData("rtgr ", double.NaN)]
    [InlineData("-1+1", 0)]
    [InlineData("1+", double.NaN)]
    [InlineData("-1/0", double.NegativeInfinity)]
    [InlineData("1+\n1", double.NaN)]
    [InlineData("11111111111111+9999999999999999999999999999999999999999999999999999999999999999999999999999999999999", 1E+85)]
    [InlineData("((5-2)/2", double.NaN)]
    [InlineData(")5+2(", double.NaN)]
    [InlineData("(-5+2)/2", -1.5)]
    [InlineData(")))))", double.NaN)]
    [InlineData("2+(*-1+0)", double.NaN)]
    [InlineData("-2", double.NaN)]
    [InlineData("5", double.NaN)]
    [InlineData("(5)", double.NaN)]
    [InlineData("-0", double.NaN)]
    public void Calculator_DataIsPassed_ReturnsResults(string actualExpression, double result)
    {
        var expectedExression = calculator.Calculate(actualExpression);

        Assert.Equal(expectedExression, result);
    }

    [Fact]
    public async Task Calculator_DataIsPassed_ReturnsLines()
    {
        string path = "./Resourses/List.txt";

        var outputPath = await calculator.CalculateFileAsync(path);

        var actualList = File.ReadAllLines(outputPath).ToList();

        var expected = new List<string>()
        {
            "2+15/3+4*2 --> 15",
            "2+2/0 --> Exception. Divide by zero.",
            "\\List.txt --> Exception. Wrong input.",
            "rtgr --> Exception. Wrong input.",
            "85484 --> Exception. Wrong input.",
            "e//// --> Exception. Wrong input.",
            "-1+1 --> 0",
            "1+ --> Exception. Wrong input.",
            "-1/0 --> Exception. Divide by zero.",
            "1+\\n1 --> Exception. Wrong input.",
            "11111111111111+9999999999999999999999999999999999999999999999999999999999999999999999999999999999999 --> 1E+85",
            "(-5+2)/2 --> -1.5",
            "((5-2)/2 --> Exception. Wrong input.",
            ")5+2( --> Exception. Wrong input.",
            ")))))))) --> Exception. Wrong input.",
            "2+(*-1+0) --> Exception. Wrong input.",
            "-2 --> Exception. Wrong input.",
            "5 --> Exception. Wrong input.",
            "(5) --> Exception. Wrong input.",
            "-0 --> Exception. Wrong input."
        };

        Assert.Equal(actualList, expected);
    }

    [Fact]
    public void Calculator_NotValidStringIsPassed_ThrowsFileNotFoundException()
    {
        string? path = string.Empty;
        Assert.ThrowsAsync<FileNotFoundException>(() => calculator.CalculateFileAsync(path));
    }
}
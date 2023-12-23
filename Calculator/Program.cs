namespace Calculator.Entities;

class Program
{
    static Calculator calculator = new Calculator();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Program can work with console input and process files, " +
                          $"choose which option you like.{Environment.NewLine}If you want to " +
                          "use Console or File press C or F respectively," +
                          " and press any key for Exit.");

        var data = Console.ReadKey();

        if (data.Key == ConsoleKey.C)
        {
            ProcessConsoleMode();
        }
        else if (data.Key == ConsoleKey.F)
        {
            await ProcessFileMode();
        }
    }

    static void ProcessConsoleMode()
    {
        Console.WriteLine();

        Console.WriteLine("Input expression here");

        string? expression = Console.ReadLine();

        ArgumentNullException.ThrowIfNull(expression);

        var result = calculator.Calculate(expression);

        if (result.Equals(double.NaN))
        {
            Console.WriteLine("{0} --> {1}", expression, $"Exception. Wrong input.");
        }
        else if (result.Equals(double.PositiveInfinity) || result.Equals(double.NegativeInfinity))
        {
            Console.WriteLine("{0} --> {1}", expression, $"Exception. Divide by zero.");
        }
        else
        {
            Console.WriteLine("{0} --> {1}", expression, result);
        }
    }

    static async Task ProcessFileMode()
    {
        Console.WriteLine();

        Console.WriteLine("Input path to file here");

        string? path = Console.ReadLine();

        ArgumentNullException.ThrowIfNull(path);

        var outputPath = await calculator.CalculateFileAsync(path);

        Console.WriteLine();

        Console.WriteLine($"File was saved in the same folder, with name {Path.GetFileName(outputPath)}");
    }
}
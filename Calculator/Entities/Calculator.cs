using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator.Entities;

public class Calculator
{
    public double Calculate(string expression)
    {
        if (!CheckLine(expression))
            return double.NaN;

        var result = CalculateExpression(expression);

        return AdditionalResultCheck(result, expression); ;
    }

    public async Task<string> CalculateFileAsync(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File with expression not found.", path);

        string? line;
        string outputPath = CreateOutputPath(path);

        using var fileStream = File.OpenRead(path);
        using var reader = new StreamReader(fileStream);

        using FileStream file = File.Create(outputPath);
        using StreamWriter sw = new StreamWriter(file);

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (CheckLine(line))
            {
                var sum = CalculateExpression(line);
                var result = AdditionalResultCheck(sum, line);

                if (result.Equals(double.PositiveInfinity) || result.Equals(double.NegativeInfinity))
                {
                    await sw.WriteLineAsync($"{line} --> Exception. Divide by zero.");
                }
                else if (result.Equals(double.NaN))
                {
                    await sw.WriteLineAsync($"{line} --> Exception. Wrong input.");
                }
                else
                {
                    await sw.WriteLineAsync($"{line} --> {result.ToString(CultureInfo.InvariantCulture)}");
                }
            }
            else
            {
                await sw.WriteLineAsync($"{line} --> Exception. Wrong input.");
            }
        }

        return outputPath;
    }

    private double CalculateExpression(string expression)
    {
        if (expression.Equals(string.Empty))
        {
            return double.NaN;
        }

        int positionCloseBracket = expression.LastIndexOf(")");
        if (positionCloseBracket >= 0)
        {
            int positionOpenBracket = expression.IndexOf("(");
            if (positionOpenBracket > positionCloseBracket || positionOpenBracket == -1)
            {
                return double.NaN;
            }
            string beforeBracket = expression.Substring(0, positionOpenBracket);
            string afterBracket = expression.Substring(positionCloseBracket + 1);
            double value = CalculateExpression(expression.Substring(positionOpenBracket + 1, positionCloseBracket - positionOpenBracket - 1));
            expression = beforeBracket + value + afterBracket;
        }

        int positionPlus = expression.LastIndexOf("+");
        if (positionPlus >= 0)
            return CalculateExpression(expression.Substring(0, positionPlus)) + CalculateExpression(expression.Substring(positionPlus + 1));

        int positionMinus = expression.LastIndexOf("-");
        if (positionMinus > 0)
        return CalculateExpression(expression.Substring(0, positionMinus)) - CalculateExpression(expression.Substring(positionMinus + 1));

        int positionMultiply = expression.LastIndexOf("*");
        if (positionMultiply >= 0)
            return CalculateExpression(expression.Substring(0, positionMultiply)) * CalculateExpression(expression.Substring(positionMultiply + 1));

        int positionDivide = expression.LastIndexOf("/");
        if (positionDivide >= 0)
            return CalculateExpression(expression.Substring(0, positionDivide)) / CalculateExpression(expression.Substring(positionDivide + 1));

        if (expression.Length > 99)
        {
            return double.NaN;
        }
        else if (double.TryParse(expression, out double result))
        {
            return result;
        }

        return double.NaN;
    }

    private string CreateOutputPath(string path)
    {
        string oldName = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);
        string NewFileName = $"{oldName}_results_{DateTime.Now.ToString("hh_mm_ss")}{extension}";
        string? directoryName = Path.GetDirectoryName(path);

        return Path.Combine(directoryName, NewFileName);
    }

    private bool CheckLine(string data)
    {
        return !Regex.IsMatch(data, @"[^\._\d\-\/\+\*\(\)]");
    }

    private double AdditionalResultCheck(double result, string expression)
    {
        if (expression == result.ToString())
        {
            return double.NaN;
        }
        else if (expression == $"({result.ToString()})")
        {
            return double.NaN;
        }

        return result;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Blazor.Shared.ExtensionMethods;
public static class StringExtensions
{
    public static int ToInt32(this string value)
    {
        return int.TryParse(value, out var result) ? result : int.MinValue;
    }

    public static string ToTitleCase(this string value)
    {
        return value switch
        {
            null => string.Empty,
            "" => string.Empty,
            _ => value.First().ToString().ToUpper() + value[1..].ToLower()
        };
    }
}

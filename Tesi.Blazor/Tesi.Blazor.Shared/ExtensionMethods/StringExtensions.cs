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
}

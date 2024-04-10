using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Blazor.Shared.Models;
public record Machine(int Id)
{
    public string Description => $"Machine {Id}";
}

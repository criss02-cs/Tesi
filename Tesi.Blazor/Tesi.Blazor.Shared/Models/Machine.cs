using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Blazor.Shared.Models;
public class Machine(int id)
{
    public Machine() : this(0)
    {
        
    }
    public int Id { get; set; } = id;
    public string Description => Id > 0 ? $"Machine {Id}" : string.Empty;
}

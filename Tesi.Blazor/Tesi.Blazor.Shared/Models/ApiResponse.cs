using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Blazor.Shared.Models;

public record ApiResponse<T>(T? Result, bool Success = true, string Message = "");
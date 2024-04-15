using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Blazor.Shared.Models;

public record ApiResponse<T>(T? Result, bool Success = true, string Message = "");


/// <summary>
/// Record per rappresentare il risultato dell'analisi, da ritornare al client per visualizzare il grafico, e la sua media
/// </summary>
/// <param name="Id">Un id per rappresentare il numero dell'esecuzione di riferimento</param>
/// <param name="Durations">La durata espressa in millisecondi</param>
public record ResultAnalysis(int Id, double Durations);
/// <summary>
/// Record per rappresentare l'insieme dei risultati, aggiungendo il solver di riferimento e la sua media
/// </summary>
/// <param name="Solver"></param>
/// <param name="Average"></param>
/// <param name="Results"></param>
public record Analysis(Solvers.Solvers Solver, double Average, List<ResultAnalysis> Results);
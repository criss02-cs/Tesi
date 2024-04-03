namespace Tesi.Blazor.Shared.Models;

public class GanttData
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Duration { get; set; }
    public int Progress { get; set; }
    public Guid? Predecessor { get; set; }
    public Guid? ParentId { get; set; }
}
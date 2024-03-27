using Tesi.Solvers;
using Tesi.Solvers.Implementations;
using Task = System.Threading.Tasks.Task;
using JobTask = Tesi.Solvers.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/assignedTasks", () =>
    {
        List<Job> _jobs =
        [
            new Job(0, [
                new JobTask(0, 3, 1),
                new JobTask(1, 2, 2),
                new JobTask(2, 2, 3),
            ]),
            new Job(1, [
                new JobTask(0, 2, 1),
                new JobTask(2, 1, 2),
                new JobTask(1, 4, 3),
            ]),
            new Job(2, [
                new JobTask(1, 4, 1),
                new JobTask(2, 3, 2),
            ]),
        ];
        var numMachines = 0;
        foreach (var job in _jobs)
        {
            foreach (var task in job.Tasks)
            {
                numMachines = Math.Max(numMachines, 1 + task.Machine);
            }
        }

        var allMachines = Enumerable.Range(0, numMachines).ToArray();

        // Computes horizon dynamically as the sum of all durations.
        var horizon = 0;
        foreach (var job in _jobs)
        {
            foreach (var task in job.Tasks)
            {
                horizon += task.Duration;
            }
        }

        var solver = new GoogleSolver();
        var result = solver.Solve(_jobs, horizon, numMachines, allMachines);
        return result;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
using PFMAssignment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ISensorDataReader dataReader = new CsvReader("counts.csv", true);
IEndPointHandler endPointHandler = new EndPointHandler(dataReader);

app.UseHttpsRedirection();
app.MapGet("/counts", GetDataPoints);

List<DataPoint> GetDataPoints(Aggregration aggregation = Aggregration.None )
{
    switch (aggregation)
    {
        case Aggregration.None: return endPointHandler.GetData();
        case Aggregration.Hourly: return endPointHandler.GetHourlyData();
        case Aggregration.Daily: return endPointHandler.GetDailyData();
        case Aggregration.Weekly: return endPointHandler.GetWeeklyData();
        default: throw new Exception("Invalid Request");
    }
};

app.Run();

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

DataPoint[] GetDataPoints(int start = 0, int end = 5)
{        
    return endPointHandler.GetData()[start..end];
};

app.Run();

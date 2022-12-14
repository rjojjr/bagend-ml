using bagend_ml.Client;
using bagend_ml.Config;
using bagend_ml.ML;
using bagend_ml.ML.Training;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EventApiConfig>(
    builder.Configuration.GetSection("EventApiConfig"));

// Add services to the container.

builder.Services.AddSingleton<EventApiRESTClient>();
builder.Services.AddSingleton<MLContextHolder>();
builder.Services.AddSingleton<TrainingModelExtractor>();
builder.Services.AddSingleton<StockOpenCloseDataLoader>();
builder.Services.AddSingleton<OpenCloseMLEngine>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


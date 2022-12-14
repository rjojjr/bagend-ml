using bagend_ml;
using bagend_ml.Client;
using bagend_ml.Config;
using bagend_ml.ML;
using bagend_ml.ML.Training;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EventApiConfig>(
    builder.Configuration.GetSection("EventApiConfig"));

// Add services to the container.

builder.Services.AddSingleton<EventApiRESTClient>();
builder.Services.AddSingleton<MLContextHolder>();
builder.Services.AddSingleton<TrainingModelExtractor>();
builder.Services.AddSingleton<StockOpenCloseDataLoader>();
builder.Services.AddSingleton<OpenCloseMLEngine>();
builder.Services.AddSingleton<Executor>();
builder.Services.AddSingleton<EventPersistenceService>();
builder.Services.AddSingleton<ModelMetaFileManager>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "bagend ML API",
        Description = "A dotnet application to manage ML"
    });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "bagend-ml.xml");
    options.IncludeXmlComments(filePath);
});

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


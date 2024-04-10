using AlgorithmAlley.DatabaseSettings;
using AlgorithmAlley.Services;
using AlgorithmAlley.Services.IServices;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MongoDB Configuration
builder.Services.Configure<AlgorithmAlleyDatabaseSettings>(builder.Configuration.GetSection("AlgorithmAlleyDatabaseSettings"));
var databaseSettings = builder.Configuration.GetSection("AlgorithmAlleyDatabaseSettings").Get<AlgorithmAlleyDatabaseSettings>();
var mongoClient = new MongoClient(databaseSettings.ConnectionString);
var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
builder.Services.AddSingleton(database);

builder.Services.AddScoped<IUserServices,UserServices>(); 

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

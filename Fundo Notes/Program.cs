

using BusinessLayer.Interface;
using BusinessLayer.Services;
using NLog.Web;
using Repository.Context;
using Repository.Interface;
using Repository.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// NLog
var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
builder.Logging.AddDebug();
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the containe
builder.Services.AddControllers();
//add scope
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IRegistrationBusinessLayer, RegistrationServiceBusinessLayer>();
builder.Services.AddScoped<IRegistrationRepositoryLayer, RegistrationServiceRepositoryLayer>();
builder.Services.AddScoped<IAuthServiceRepositoryLayer, AuthServiceRepositoryLayer>();
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

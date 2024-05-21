using BusinessLayer.Interface;
using BusinessLayer.Services;
using BussinesLayer.Interface;
using BussinesLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ModelLayer.Email;
using NLog.Web;
using Repository.Context;
using Repository.Interface;
using Repository.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using RepositoryLayer.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// NLog
var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
builder.Logging.AddDebug();
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the containe
builder.Services.AddControllers();
//add scope
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserBusinessLayer, RegistrationServiceBusinessLayer>();
builder.Services.AddScoped<IRegistrationRepositoryLayer, UserServiceRepositoryLayer>();
builder.Services.AddScoped<IAuthServiceRepositoryLayer, AuthServiceRepositoryLayer>();

builder.Services.AddScoped<IEmailRepositoryLayer, EmailServiceRepositoryLayer>();
builder.Services.AddScoped<IEmailBusinessLayer, EmailServiceBusinessLayer>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddScoped<INoteBusinessLayer, NoteServiceBusinessLayer>();
builder.Services.AddScoped<INotesRepositoryLayer, NoteServiceRepositoryLayer>();
builder.Services.AddScoped<ILabelRepositoryLayer, LabelServiceRepositoryLayer>();
builder.Services.AddScoped<ILabelBusinessLayer, LabelRepositoryBusinessLayer>();
builder.Services.AddScoped<ICollaborationRL, CollaborationService>();
builder.Services.AddScoped<ICollaboration, CollaborationServicebl>();

// Add Redis distributed cache
builder.Services.AddSingleton<IDistributedCache, CacheService>(sp =>
{
    var redisConfiguration = builder.Configuration.GetSection("Redis")["RedisConnection"];
    return new CacheService(redisConfiguration);
});


//builder.Services.AddDistributedMemoryCache();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]);

// Add authentication services with JWT Bearer token validation to the service collection
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)



    // Add JWT Bearer authentication options
    .AddJwtBearer(options =>
    {
        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Specify whether the server should validate the signing key
            ValidateIssuerSigningKey = true,

            // Set the signing key to verify the JWT signature
            IssuerSigningKey = new SymmetricSecurityKey(key),

            // Specify whether to validate the issuer of the token (usually set to false for development)
            ValidateIssuer = false,

            // Specify whether to validate the audience of the token (usually set to false for development)
            ValidateAudience = false,
        };
    });



// Configure Swagger/OpenAPI
// Configure Swagger generation options
builder.Services.AddSwaggerGen(c =>
{
    // Define Swagger document metadata (title and version)
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configure JWT authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Describe how to pass the token
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization", // The name of the header containing the JWT token
        In = ParameterLocation.Header, // Location of the JWT token in the request headers
        Type = SecuritySchemeType.Http, // Specifies the type of security scheme (HTTP in this case)
        Scheme = "bearer", // The authentication scheme to be used (in this case, "bearer")
        BearerFormat = "JWT" // The format of the JWT token
    });

    // Specify security requirements for Swagger endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            // Define a reference to the security scheme defined above
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // The ID of the security scheme (defined in AddSecurityDefinition)
                }
            },
            new string[] {} // Specify the required scopes (in this case, none)
        }
    });
});

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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using SensorDataApi.BackgroundServices;
using SensorDataApi.Data;
using SensorDataApi.Data.Repositories;
using SensorDataApi.Data.SeedData;
using SensorDataApi.Data.UnitOfWork;
using SensorDataApi.Middlewares;
using SensorDataApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();

//Depeendency Injection
builder.Services.AddScoped<ILightSensorRepository, LightSensorRepository>();
builder.Services.AddScoped<ILightSensorService, LightSensorService>();
builder.Services.AddScoped<ITempSensorRepository, TempSensorRepository>();
builder.Services.AddScoped<ITempSensorService, TempSensorService>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//builder.Services.AddScoped<DbInitializer>();//Use that to initialize data to test get method

builder.Services.AddMemoryCache();
builder.Services.AddLogging();

// Hosted Service which runs our simulators
builder.Services.AddHostedService<SimulatorsBackgroundService>();

builder.Services.AddDbContext<SensorDataDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sensor Data Api", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Oauth2.0 which uses AuthorizationCode flow",
            Name = "oauth2.0",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(builder.Configuration["swaggerAzureAd:AuthorizationUrl"]),
                    TokenUrl = new Uri(builder.Configuration["swaggerAzureAd:TokenUrl"]),
                    Scopes = new Dictionary<string, string>
                    {
                        {builder.Configuration["SwaggerAzureAd:Scope"],"Access API as User" }
                    }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference= new OpenApiReference{Type = ReferenceType.SecurityScheme,Id="oauth2"}
                },
                new[]{ builder.Configuration["SwaggerAzureAd:Scope"] }
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(builder.Configuration["SwaggerAzureAd:ClientId"]);
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

//Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();


app.UseAuthorization();

//#region SeedData
//using (var scope = app.Services.CreateScope())  // Uncomment that if you want seed data and uncomment service too
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var dataSeeder = services.GetRequiredService<DbInitializer>();
//        await dataSeeder.InitializeAsync();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("An error occurred while seeding the database: " + ex.Message);
//    }
//}
//#endregion

app.MapControllers();

app.Run();

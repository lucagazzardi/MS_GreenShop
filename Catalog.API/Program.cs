using Catalog.API.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Catalog.API.Business.Interfaces;
using Catalog.API.Business;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using RabbitMQ.Client;
using RabbitMQService.cs.EventsCollection;
using RabbitMQService.cs.Infrastructure.Interfaces;
using RabbitMQService.cs.Infrastructure;
using RabbitMQService.cs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .MinimumLevel.Verbose()
  .Enrich.WithProperty("AppName", builder.Configuration["AppName"])
  .WriteTo.File("logs/catalog-.log", Serilog.Events.LogEventLevel.Verbose, builder.Configuration["SerilogOutputTemplate"], rollingInterval: RollingInterval.Day)
  .WriteTo.Seq(builder.Configuration["SeqUrl"])
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers();
builder.Services
    .AddDbContext<CatalogContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb"));
    });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Authorization

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

#endregion

builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddAutoMapper(typeof(Program));

RegisterRabbitMQ();

var app = builder.Build();

CreateDbIfNotExists(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void CreateDbIfNotExists(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<CatalogContext>();
            new CatalogContextSeed().Seed(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}

void RegisterRabbitMQ()
{
    builder.Services.AddSingleton<IRabbitMQConnection>(sp =>
    {

        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBusConnection"],
            DispatchConsumersAsync = true
        };

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"]))
        {
            factory.UserName = builder.Configuration["EventBusUserName"];
        }

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"]))
        {
            factory.Password = builder.Configuration["EventBusPassword"];
        }

        return new DefaultRabbitMQConnection(factory);
    });

    builder.Services.AddSingleton<IRabbitMQManager, RabbitMQManager>(sp =>
    {
        var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQConnection>();
        var eventBusSubscriptionsManager = sp.GetRequiredService<IEventSubscriptionManager>();
        var logger = sp.GetRequiredService<ILogger<RabbitMQManager>>();


        return new RabbitMQManager(rabbitMQPersistentConnection, eventBusSubscriptionsManager, sp, logger, subscriptionClientName);
    });

    builder.Services.AddSingleton<IEventSubscriptionManager, SubscriptionManager>();
}

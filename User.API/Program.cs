using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using User.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using User.API.Repository;
using RabbitMQ.Client;
using RabbitMQService.cs.Infrastructure.Interfaces;
using RabbitMQService.cs.Infrastructure;
using RabbitMQService.cs;
using User.API.EventHandling;
using RabbitMQService.cs.EventsCollection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services
    .AddDbContext<UserContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb"));
    });

builder.Services.AddIdentity<User.Model.User, IdentityRole>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 1;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UserContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

RegisterRabbitMQ();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region JWT

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

#region DI

builder.Services.AddTransient<IUserService, UserService>();

#endregion

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

ConfigureRabbitMQ(app);

app.Run();

static void CreateDbIfNotExists(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<UserContext>();
            new UserContextSeed().Seed(context);
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

        return new RabbitMQManager(rabbitMQPersistentConnection, eventBusSubscriptionsManager, sp, subscriptionClientName);
    });

    builder.Services.AddSingleton<IEventSubscriptionManager, SubscriptionManager>();

    builder.Services.AddTransient<AddedNewProductEventHandler>();
}

void ConfigureRabbitMQ(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IRabbitMQManager>();

    eventBus.Subscribe<AddedNewProductEvent, AddedNewProductEventHandler>();
}

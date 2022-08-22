using CusomerManagement.Persistance;
using FullDockerTest.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);

builder.Services.AddDbContext<AppDbContext>(options => options
               .UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"),
               b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

#region Masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SagaCustomerValidateConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/",
        h =>
        {
            h.Username(BusConstants.Username);
            h.Password(BusConstants.Password);
        });
    });
});

//builder.Services.AddMassTransitHostedService();

#endregion


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


#region Migration
using var serviceProvider = builder.Services.BuildServiceProvider();
try
{
    var dbContext = serviceProvider.GetService<AppDbContext>();
    if (dbContext.Database.IsSqlServer())
    {
        dbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = serviceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

    throw;
}


#endregion


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

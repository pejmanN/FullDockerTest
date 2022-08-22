using FullDockerTest.Infra.Consumers;
using FullDockerTest.Infra.Persistence;
using FullDockerTest.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Saga;
using Shared.Saga.Courier;
using Shared.Saga.Models;
using Shared.Saga.OrderStateMachineActivities;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddScoped<OrderAcceptedActivity>();
builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<FulfillOrderConsumer>(); 
    //x.AddConsumer<SagaStartOrderConsumer>();
    //x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<RoutingSlipEventConsumer>();

    //should pull out of AddMassTransit, it is not related to MassTransit
    builder.Services.AddDbContext<OrderDbContext>(options => options
               .UseSqlServer(builder.Configuration.GetConnectionString("orderConn"),
               b => b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

    x.AddConsumersFromNamespaceContaining<SagaStartOrderConsumer>();
    x.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>(); //Add Courier Activities

    x.AddRequestClient<IGetInfoRequest>(
                  new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<GetInfoConsumer>()}"));
    x.AddRequestClient<IAllocateInventory>();
    x.AddRequestClient<ICheckOrderStatus>();

    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
          .EntityFrameworkRepository(r =>
          {
              r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

              r.AddDbContext<DbContext, OrderStateDbContext>((provider, dbContextOptionBuilder) =>
              {
                  dbContextOptionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("orderConn"), m =>
                  {
                      m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                      m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                  });
              });
          });


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context); //create and configure receiver endpoints
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/",
            h =>
            {
                h.Username(BusConstants.Username);
                h.Password(BusConstants.Password);
            }
        );
    });
});

//builder.Services.AddMassTransitHostedService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Migration
using var serviceProvider = builder.Services.BuildServiceProvider();
try
{
    var orderStateDbContext = serviceProvider.GetService<DbContext>();
    if (orderStateDbContext.Database.IsSqlServer())
    {
        orderStateDbContext.Database.Migrate();
    }

    var orderAppDbContext = serviceProvider.GetService<OrderDbContext>();
    if (orderAppDbContext.Database.IsSqlServer())
    {
        orderAppDbContext.Database.Migrate();
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

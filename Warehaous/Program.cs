using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Shared;
using Shared.Saga.AllocationStateMachine;
using System.Reflection;
using Warehaous.Infra.Consumers;
using Warehaous.Infra.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddQuartz(q =>
{
    q.SchedulerName = "MassTransit-Scheduler";
    q.SchedulerId = "AUTO";

    q.UseMicrosoftDependencyInjectionJobFactory();

    q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

    q.UseTimeZoneConverter();

    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.RetryInterval = TimeSpan.FromSeconds(15);

        s.UseSqlServer(builder.Configuration.GetConnectionString("quartz"));

        s.UseJsonSerializer();

        s.UseClustering(c =>
        {
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
            c.CheckinInterval = TimeSpan.FromSeconds(10);
        });
    });
});

builder.Services.Configure<QuartzEndpointOptions>(builder.Configuration.GetSection("QuartzEndpoint"));
builder.Services.AddQuartzHostedService(options =>
{
    options.StartDelay = TimeSpan.FromSeconds(5);
    options.WaitForJobsToComplete = true;
});

#region Masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddQuartzConsumers();
    x.AddConsumersFromNamespaceContaining<AllocateInventoryConsumer>();
    x.AddSagaStateMachine<AllocationStateMachine, AllocationState>()
         .EntityFrameworkRepository(r =>
         {
             r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

             r.AddDbContext<DbContext, AllocationStateDbContext>((provider, dbContextOptionBuilder) =>
             {
                 dbContextOptionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("WarehouseConn"), m =>
                 {
                     m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                     m.MigrationsHistoryTable($"__{nameof(AllocationStateDbContext)}");
                 });
             });
         });

    Uri schedulerEndpoint = new Uri("queue:quartz");

    x.UsingRabbitMq((context, cfg) =>
    {
        //for configuring Quartz endpoint
        cfg.UseMessageScheduler(schedulerEndpoint);


        //cfg.ReceiveEndpoint("scheduler", endpoint =>
        //{
        //    cfg.UseMessageScheduler(schedulerEndpoint);
        //});

        cfg.ConfigureEndpoints(context);
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/",
        h =>
        {
            h.Username(BusConstants.Username);
            h.Password(BusConstants.Password);
        });
    });
});

#endregion

var app = builder.Build();

#region Migration
using var serviceProvider = builder.Services.BuildServiceProvider();
try
{
    var allocationStateDbContext = serviceProvider.GetService<DbContext>();
    if (allocationStateDbContext.Database.IsSqlServer())
    {
        allocationStateDbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = serviceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

    throw;
}
#endregion

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

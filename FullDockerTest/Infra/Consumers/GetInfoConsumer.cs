using FullDockerTest.Models;
using MassTransit;
using Shared;

namespace FullDockerTest.Infra.Consumers
{
    public class GetInfoConsumer : IConsumer<IGetInfoRequest>
    {
        readonly ILogger<GetInfoConsumer> _logger;

        public GetInfoConsumer()
        { }
        public GetInfoConsumer(ILogger<GetInfoConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IGetInfoRequest> context)
        {
            _logger.LogInformation("pejmannn--StartOrderConsumer-- Order" +
                $" Transation Started and event published: {context.Message.Name}");

            if (context.Message.Name == "test")
            {
                await context.RespondAsync<GetInfoAccepted>(new
                {
                    Name = context.Message.Name,
                });

                return;
            }

            await context.RespondAsync<GetInfoReject>(new
            {
                Message = "we just accept test",
            });

        }
    }
}
